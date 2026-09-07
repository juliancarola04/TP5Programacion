using System.Data.Common;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;

namespace API.Services
{
    public class ProductoService
    {
        private readonly IProductoRepository _repo;

        public ProductoService(IProductoRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ProductoListadoDtoOutput>> ObtenerTodos()
        {
            try
            {
                List<Producto> productos = await _repo.ObtenerTodos();

                return productos.Select(p => new ProductoListadoDtoOutput(
                    p.Id, p.Nombre, p.PrecioCompra, p.PrecioVenta, p.Stock, p.CategoriaId
                )).ToList();
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task<ProductoDetalleDtoOutput> ObtenerPorId(int id)
        {
            try
            {
                Producto? producto = await _repo.ObtenerPorId(id);

                if (producto is null)
                {
                    throw new RecursoNoExisteException("No existe ningún producto con ese id.");
                }

                return new ProductoDetalleDtoOutput(
                    producto.Id, producto.Nombre, producto.PrecioCompra, producto.PrecioVenta,
                    producto.Stock, producto.CategoriaId, producto.Categoria?.Nombre,
                    producto.Imagen is null ? null : new ImagenDtoOutput(
                        producto.Imagen.Id, producto.Imagen.NombreOriginal, producto.Imagen.NombreArchivo,
                        producto.Imagen.RutaRelativa, producto.Imagen.TipoContenido,
                        producto.Imagen.TamanoBytes, producto.Imagen.FechaCreacion)
                );
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task<ProductoListadoDtoOutput> Crear(CrearProductoDtoInput dto)
        {
            if (!Validaciones.Requeridos(dto.Nombre))
            {
                throw new DatosLlegaronErradosException("El nombre del producto es obligatorio.");
            }

            try
            {
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

                return new ProductoListadoDtoOutput(
                    producto.Id, producto.Nombre, producto.PrecioCompra,
                    producto.PrecioVenta, producto.Stock, producto.CategoriaId);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarProductoDtoInput dto)
        {
            if (!Validaciones.Requeridos(dto.Nombre))
            {
                throw new DatosLlegaronErradosException("El nombre del producto es obligatorio.");
            }

            try
            {
                Producto? producto = await _repo.ObtenerPorId(id);

                if (producto is null)
                {
                    throw new RecursoNoExisteException("No existe ningún producto con ese id.");
                }

                producto.Nombre = dto.Nombre;
                producto.PrecioCompra = dto.PrecioCompra;
                producto.PrecioVenta = dto.PrecioVenta;
                producto.Stock = dto.Stock;
                producto.CategoriaId = dto.CategoriaId;

                await _repo.Actualizar(producto);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                Producto? producto = await _repo.ObtenerPorId(id);

                if (producto is null)
                {
                    throw new RecursoNoExisteException("No existe ningún producto con ese id.");
                }

                await _repo.Eliminar(producto);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
    }
}
