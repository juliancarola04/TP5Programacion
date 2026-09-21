using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using TP5Programacion.Compartidas.DTO.Producto.Request;
using TP5Programacion.Compartidas.DTO.Producto.Response;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Producto;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Producto;

namespace API.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _repo;
        private readonly ICategoriaRepository _categoriaRepository;

        public ProductoService(IProductoRepository repo, ICategoriaRepository categoriaRepository)
        {
            _repo = repo;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<PaginadoResponse<ProductoListadoResponse>> ObtenerTodos(ParametroPaginacionProductoRequest parametros)
        {

                int numeroPagina = parametros.NumeroPagina is null || parametros.NumeroPagina < 1
                    ? 1
                    : parametros.NumeroPagina.Value;

                int tamanoPagina = parametros.TamanoPagina is null || parametros.TamanoPagina < 1
                    ? 20
                    : parametros.TamanoPagina > 50 ? 50 : parametros.TamanoPagina.Value;

                int? categoriaId = parametros.CategoriaId;

                string? direccion =
                    string.IsNullOrWhiteSpace(parametros.Direccion) ||
                    parametros.Direccion?.ToLower() is not ("asc" or "desc")
                        ? "desc"
                        : parametros.Direccion;

                string? buscar = parametros.Buscar?.Trim().ToLower();

                string? ordenarPor = string.IsNullOrWhiteSpace(parametros.OrdenarPor) ? null : parametros.OrdenarPor;

                ProductoQueryParametros productoQueryParametros = new ProductoQueryParametros
                {
                    NumeroPagina = numeroPagina,
                    TamanoPagina = tamanoPagina,
                    CategoriaId = categoriaId,
                    Buscar = buscar,
                    OrdenarPor = ordenarPor,
                    Direccion = direccion
                };

                PaginadoResponse<Producto> resultado = await _repo.ObtenerTodos(productoQueryParametros);

                List<ProductoListadoResponse> productos = resultado.Datos.Select(p => new ProductoListadoResponse(
                    p.Id, p.Nombre, p.PrecioCompra, p.PrecioVenta, p.Stock, p.CategoriaId
                )).ToList();

                return new PaginadoResponse<ProductoListadoResponse>(
                    productos,
                    resultado.NumeroPagina,
                    resultado.TamanoPagina,
                    resultado.TotalRegistros
                );

        }

        public async Task<Producto> ObtenerPorId(int id)
        {
                Producto? producto = await _repo.ObtenerPorId(id);

                if (producto is null)
                {
                    throw new RecursoNoExisteException("No existe ningún producto con ese id.");
                }

                return producto;
        }

        public async Task<ProductoListadoResponse> Crear(CrearProductoRequest dto)
        {
            
            Categoria? categoria = await _categoriaRepository.ObtenerPorId(dto.CategoriaId);

            if (categoria is null)
            {
                throw new RecursoNoExisteException("No existe ninguna categoría con ese id.");
            }
            
            if (Validaciones.EstanDatosBien(dto.Nombre) == false)
            {
                throw new DatosLlegaronErradosException("El nombre del producto es obligatorio.");
            }

            if (Validaciones.EstanDatosBien(dto.PrecioCompra, dto.PrecioVenta, dto.Stock) == false)
            {
                throw new DatosLlegaronErradosException("Tanto el precio de compra, como el de venta y del stock son obligatorios");
            }

            if (dto.PrecioVenta < 0 || dto.PrecioCompra < 0 || dto.Stock < 0)
            {
                throw new DatosLlegaronErradosException("Tanto el precio de compra, como el de venta y del stock no pueden ser negativos");
            }

                if (await _repo.ExistePorNombre(dto.Nombre))
                {
                    throw new RecursoExistenteException("Ya existe un producto con ese nombre.");
                }

                Producto producto = new Producto
                {
                    Nombre = dto.Nombre,
                    PrecioCompra = dto.PrecioCompra,
                    PrecioVenta = dto.PrecioVenta,
                    Stock = dto.Stock,
                    CategoriaId = dto.CategoriaId
                };

                await _repo.Crear(producto);

                return new ProductoListadoResponse(
                    producto.Id, producto.Nombre, producto.PrecioCompra,
                    producto.PrecioVenta, producto.Stock, producto.CategoriaId);
        }

        public async Task Actualizar(int id, ActualizarProductoRequest dto)
        {
            if (Validaciones.EstanDatosBien(dto.Nombre) == false)
            {
                throw new DatosLlegaronErradosException("El nombre del producto es obligatorio.");
            }
            
            if (Validaciones.EstanDatosBien(dto.PrecioCompra, dto.PrecioVenta, dto.Stock) == false)
            {
                throw new DatosLlegaronErradosException("Tanto el precio de compra, como el de venta y del stock son obligatorios");
            }

            if (dto.PrecioVenta < 0 || dto.PrecioCompra < 0 || dto.Stock < 0)
            {
                throw new DatosLlegaronErradosException("Tanto el precio de compra, como el de venta y del stock no pueden ser negativos");
            }

                Producto? producto = await _repo.ObtenerPorId(id);

                if (producto is null)
                {
                    throw new RecursoNoExisteException("No existe ningún producto con ese id.");
                }

                if (producto.Nombre != dto.Nombre && await _repo.ExistePorNombre(producto.Nombre))
                {
                    throw new RecursoExistenteException("Ya existe un producto con ese nombre.");
                }

                Categoria? categoria = await _categoriaRepository.ObtenerPorId(dto.CategoriaId);

                if (categoria is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna categoría con ese id.");
                }

                producto.Nombre = dto.Nombre;
                producto.PrecioCompra = dto.PrecioCompra;
                producto.PrecioVenta = dto.PrecioVenta;
                producto.Stock = dto.Stock;
                producto.CategoriaId = dto.CategoriaId;

                await _repo.Actualizar(producto);
        }

        public async Task Eliminar(int id)
        {

                Producto? producto = await _repo.ObtenerPorId(id);

                if (producto is null)
                {
                    throw new RecursoNoExisteException("No existe ningún producto con ese id.");
                }

                await _repo.Eliminar(producto);
        }
    }
}
