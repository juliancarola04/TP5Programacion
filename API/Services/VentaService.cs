using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using System.Data.Common;

namespace API.Services
{
    public class VentaService
    {
        private readonly IVentaRepository _repo;
        private readonly IProductoRepository _productoRepo;
        private readonly IClienteRepository _clienteRepo;

        public VentaService(IVentaRepository repo, IProductoRepository productoRepo, IClienteRepository clienteRepo)
        {
            _repo = repo;
            _productoRepo = productoRepo;
            _clienteRepo = clienteRepo;
        }
        public async Task<List<VentaListadoDtoOutput>> ObtenerTodas()
        {
            try
            {
                List<Venta> ventas = await _repo.ObtenerTodas();

                return ventas.Select(v => new VentaListadoDtoOutput(
                    v.Id, v.Fecha, v.Total, v.ClienteId, v.Cliente.Nombre, v.Anulada
                )).ToList();
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task<VentaDtoOutput> ObtenerPorId(int id)
        {
            try
            {
                Venta? venta = await _repo.ObtenerPorId(id);

                if (venta is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna venta con ese id.");
                }

                return MapearADto(venta);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task<VentaDtoOutput> Crear(CrearVentaDtoInput dto, int usuarioId)
        {
            if (dto.Items is null || dto.Items.Count == 0)
            {
                throw new DatosLlegaronErradosException("La venta debe tener al menos un producto.");
            }

            if (dto.Items.Any(i => i.Cantidad <= 0))
            {
                throw new DatosLlegaronErradosException("La cantidad de cada producto debe ser mayor a cero.");
            }

            try
            {
                Cliente? cliente = await _clienteRepo.ObtenerPorId(dto.ClienteId);
                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                // Agrupamos por si mandaron el mismo producto repetido en la lista.
                var itemsAgrupados = dto.Items
                    .GroupBy(i => i.ProductoId)
                    .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
                    .ToList();

                List<DetalleVenta> detalles = new List<DetalleVenta>();
                decimal total = 0;

                foreach (var item in itemsAgrupados)
                {
                    Producto? producto = await _productoRepo.ObtenerPorId(item.ProductoId);

                    if (producto is null)
                    {
                        throw new RecursoNoExisteException($"No existe ningún producto con id {item.ProductoId}.");
                    }

                    if (producto.Stock < item.Cantidad)
                    {
                        throw new DatosLlegaronErradosException(
                            $"Stock insuficiente para el producto '{producto.Nombre}' (id {producto.Id}). " +
                            $"Disponible: {producto.Stock}, solicitado: {item.Cantidad}.");
                    }

                    producto.Stock -= item.Cantidad; // entidad trackeada, se guarda junto con la venta

                    decimal subtotal = producto.PrecioVenta * item.Cantidad;
                    total += subtotal;

                    detalles.Add(new DetalleVenta
                    {
                        ProductoId = producto.Id,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.PrecioVenta
                    });
                }

                Venta venta = new Venta
                {
                    Fecha = DateTime.UtcNow,
                    Total = total,
                    ClienteId = dto.ClienteId,
                    UsuarioId = usuarioId,
                    DetallesVentas = detalles
                };

                await _repo.Crear(venta);

                Venta? ventaCompleta = await _repo.ObtenerPorId(venta.Id);
                return MapearADto(ventaCompleta!);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        private static VentaDtoOutput MapearADto(Venta venta)
        {
            List<DetalleVentaDtoOutput> detalles = venta.DetallesVentas.Select(d => new DetalleVentaDtoOutput(
                d.ProductoId,
                d.Producto.Nombre,
                d.Cantidad,
                d.PrecioUnitario,
                d.PrecioUnitario * d.Cantidad
            )).ToList();

            return new VentaDtoOutput(
                venta.Id,
                venta.Fecha,
                venta.Total,
                venta.ClienteId,
                venta.Cliente.Nombre,
                venta.UsuarioId,
                venta.Usuario.Username,
                venta.Anulada,
                detalles);
        }
        public async Task Anular(int id)
        {
            try
            {
                Venta? venta = await _repo.ObtenerParaAnular(id);

                if (venta is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna venta con ese id.");
                }

                if (venta.Anulada)
                {
                    throw new DatosLlegaronErradosException("La venta ya se encuentra anulada.");
                }

                foreach (DetalleVenta detalle in venta.DetallesVentas)
                {
                    detalle.Producto.Stock += detalle.Cantidad;
                }

                venta.Anulada = true;

                await _repo.GuardarCambios();
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
    }
}
