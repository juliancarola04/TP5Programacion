using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;
using TP5Programacion.Compartidas.DTO.Ingreso.Request;
using TP5Programacion.Compartidas.DTO.Ingreso.Response;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Ingreso;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Ingreso;

namespace API.Services;

public class IngresoService
{
    private readonly IIngresoRepository _repo;
    private readonly IProductoRepository _productoRepo;
    private readonly IProveedorRepository _proveedorRepo;

    public IngresoService(IIngresoRepository repo, IProductoRepository productoRepo, IProveedorRepository proveedorRepo)
    {
        _repo = repo;
        _productoRepo = productoRepo;
        _proveedorRepo = proveedorRepo;
    }

    public async Task<PaginadoResponse<IngresoListadoResponse>> ObtenerTodos(ParametroPaginacionIngresoRequest parametros)
    {
        try
        {
            int numeroPagina = parametros.NumeroPagina is null || parametros.NumeroPagina < 1
                ? 1
                : parametros.NumeroPagina.Value;

            int tamanoPagina = parametros.TamanoPagina is null || parametros.TamanoPagina < 1
                ? 20
                : parametros.TamanoPagina > 50 ? 50 : parametros.TamanoPagina.Value;

            int? proveedorId = parametros.ProveedorId;
            bool? anulado = parametros.Anulado;

            string? direccion =
                string.IsNullOrWhiteSpace(parametros.Direccion) &&
                parametros.Direccion?.ToLower() is not ("asc" or "desc")
                    ? "desc"
                    : parametros.Direccion;

            string? buscar = parametros.Buscar?.Trim().ToLower();

            string? ordenarPor = string.IsNullOrWhiteSpace(parametros.OrdenarPor) ? "fecha" : parametros.OrdenarPor;

            IngresoQueryParametros ingresoQueryParametros = new IngresoQueryParametros
            {
                NumeroPagina = numeroPagina,
                TamanoPagina = tamanoPagina,
                ProveedorId = proveedorId,
                Anulado = anulado,
                Buscar = buscar,
                OrdenarPor = ordenarPor,
                Direccion = direccion
            };

            PaginadoResponse<Ingreso> resultado = await _repo.ObtenerTodos(ingresoQueryParametros);

            List<IngresoListadoResponse> ingresos = resultado.Datos.Select(i => new IngresoListadoResponse(
                i.Id, i.Fecha, i.Total, i.ProveedorId, i.Proveedor.RazonSocial, i.Anulado
            )).ToList();

            return new PaginadoResponse<IngresoListadoResponse>(
                ingresos,
                resultado.NumeroPagina,
                resultado.TamanoPagina,
                resultado.TotalRegistros
            );
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<IngresoResponse> ObtenerPorId(int id)
    {
        try
        {
            Ingreso? ingreso = await _repo.ObtenerPorId(id);

            if (ingreso is null)
            {
                throw new RecursoNoExisteException("No existe ningún ingreso con ese id.");
            }

            return MapearADto(ingreso);
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<IngresoResponse> Crear(CrearIngresoRequest dto, int usuarioId)
    {
        if (dto.Items is null || dto.Items.Count == 0)
        {
            throw new DatosLlegaronErradosException("El ingreso debe tener al menos un producto.");
        }

        if (dto.Items.Any(i => i.Cantidad <= 0))
        {
            throw new DatosLlegaronErradosException("La cantidad de cada producto debe ser mayor a cero.");
        }

        if (dto.Items.Any(i => i.PrecioUnitario <= 0))
        {
            throw new DatosLlegaronErradosException("El precio unitario de cada producto debe ser mayor a cero.");
        }

        bool hayDuplicados = dto.Items
            .GroupBy(i => i.ProductoId)
            .Any(g => g.Count() > 1);

        if (hayDuplicados)
        {
            throw new DatosLlegaronErradosException(
                "Hay un producto repetido en la lista de items. Combiná las cantidades en un solo ítem antes de enviar.");
        }

        try
        {
            Proveedor? proveedor = await _proveedorRepo.BuscarPorId(dto.ProveedorId);
            if (proveedor is null)
            {
                throw new RecursoNoExisteException("No existe ningún proveedor con ese id.");
            }

            List<DetalleIngreso> detalles = new List<DetalleIngreso>();
            decimal total = 0;

            foreach (CrearIngresoItemRequest item in dto.Items)
            {
                Producto? producto = await _productoRepo.ObtenerPorId(item.ProductoId);

                if (producto is null)
                {
                    throw new RecursoNoExisteException($"No existe ningún producto con id {item.ProductoId}.");
                }

                producto.Stock += item.Cantidad;
                producto.PrecioCompra = item.PrecioUnitario; // actualiza el costo vigente en el catálogo

                decimal subtotal = item.PrecioUnitario * item.Cantidad;
                total += subtotal;

                detalles.Add(new DetalleIngreso
                {
                    ProductoId = producto.Id,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = item.PrecioUnitario
                });
            }

            Ingreso ingreso = new Ingreso
            {
                Fecha = DateTime.UtcNow,
                Total = total,
                ProveedorId = dto.ProveedorId,
                UsuarioId = usuarioId,
                DetallesIngresos = detalles
            };

            await _repo.Crear(ingreso);

            Ingreso? ingresoCompleto = await _repo.ObtenerPorId(ingreso.Id);
            return MapearADto(ingresoCompleto!);
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task Anular(int id)
    {
        try
        {
            Ingreso? ingreso = await _repo.ObtenerParaAnular(id);

            if (ingreso is null)
            {
                throw new RecursoNoExisteException("No existe ningún ingreso con ese id.");
            }

            if (ingreso.Anulado)
            {
                throw new DatosLlegaronErradosException("El ingreso ya se encuentra anulado.");
            }

            // Antes de tocar nada, verificamos que revertir el stock no deje ningún producto en negativo.
            foreach (DetalleIngreso detalle in ingreso.DetallesIngresos)
            {
                if (detalle.Producto.Stock < detalle.Cantidad)
                {
                    throw new DatosLlegaronErradosException(
                        $"No se puede anular: el producto '{detalle.Producto.Nombre}' (id {detalle.Producto.Id}) " +
                        $"tiene stock actual {detalle.Producto.Stock}, menor a las {detalle.Cantidad} unidades a revertir.");
                }
            }

            foreach (DetalleIngreso detalle in ingreso.DetallesIngresos)
            {
                detalle.Producto.Stock -= detalle.Cantidad;
            }

            ingreso.Anulado = true;

            await _repo.GuardarCambios();
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    private static IngresoResponse MapearADto(Ingreso ingreso)
    {
        List<IngresoDetalleResponse> detalles = ingreso.DetallesIngresos.Select(d => new IngresoDetalleResponse(
            d.ProductoId,
            d.Producto.Nombre,
            d.Cantidad,
            d.PrecioUnitario,
            d.PrecioUnitario * d.Cantidad
        )).ToList();

        return new IngresoResponse(
            ingreso.Id,
            ingreso.Fecha,
            ingreso.Total,
            ingreso.ProveedorId,
            ingreso.Proveedor.RazonSocial,
            ingreso.UsuarioId,
            ingreso.Usuario.Username,
            ingreso.Anulado,
            detalles);
    }
}
