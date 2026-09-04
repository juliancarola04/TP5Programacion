using API.Data;
using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly DataContext dataContext;
        private readonly IWebHostEnvironment _env;
        // Extensiones y tipos MIME válidos (JPG y PNG)
        private readonly string[] _extensionesPermitidas = [".jpg", ".jpeg", ".png"];
        private readonly string[] _tiposMimePermitidos = ["image/jpeg", "image/png"];
        private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB máximo
        public ProductosController(DataContext dataContext, IWebHostEnvironment env)
        {
            this.dataContext = dataContext;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductoDetalleDTO>>> GetProductos()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var productos = await dataContext.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .ToListAsync();

            var resultado = productos.Select(p => new ProductoDetalleDTO(
                p.Id,
                p.Nombre,
                p.PrecioCompra,
                p.PrecioVenta,
                p.Stock,
                p.CategoriaId,
                p.Categoria.Nombre,
                p.Imagen == null
                    ? null
                    : new ImagenResponseDTO(
                        p.Imagen.Id,
                        p.Imagen.NombreOriginal,
                        p.Imagen.NombreArchivo,
                        $"{baseUrl}/{p.Imagen.RutaRelativa}",
                        p.Imagen.TipoContenido,
                        p.Imagen.TamanoBytes,
                        p.Imagen.FechaCreacion
                    )
            ));

            return Ok(resultado);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductoDetalleDTO>> GetProducto(int id)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var producto = await dataContext.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .Where(p => p.Id == id)
                .Select(p => new ProductoDetalleDTO(
                    p.Id,
                    p.Nombre,
                    p.PrecioCompra,
                    p.PrecioVenta,
                    p.Stock,
                    p.CategoriaId,
                    p.Categoria.Nombre,
                    p.Imagen == null
                        ? null
                        : new ImagenResponseDTO(
                            p.Imagen.Id,
                            p.Imagen.NombreOriginal,
                            p.Imagen.NombreArchivo,
                            $"{baseUrl}/{p.Imagen.RutaRelativa}",
                            p.Imagen.TipoContenido,
                            p.Imagen.TamanoBytes,
                            p.Imagen.FechaCreacion
                        )
                ))
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ListaProductosDTO>> CrearProducto(CrearProductoDTO dto)
        {
            var categoriaExiste = await dataContext.Categorias
                .AnyAsync(c => c.Id == dto.CategoriaID);

            if (!categoriaExiste)
            {
                return BadRequest("La categoría indicada no existe.");
            }

            var producto = new Producto
            {
                Nombre = dto.Nombre,
                PrecioCompra = dto.PrecioCompra,
                PrecioVenta = dto.PrecioVenta,
                Stock = dto.Stock,
                CategoriaId = dto.CategoriaID
            };

            dataContext.Productos.Add(producto);
            await dataContext.SaveChangesAsync();

            var resultado = new ListaProductosDTO(
                producto.Id,
                producto.Nombre,
                producto.PrecioCompra,
                producto.PrecioVenta,
                producto.Stock,
                producto.CategoriaId
            );

            return CreatedAtAction(
                nameof(GetProducto),
                new { id = producto.Id },
                resultado
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProducto(int id, CrearProductoDTO dto)
        {
            var productoExistente = await dataContext.Productos
                .FindAsync(id);

            if (productoExistente == null)
            {
                return NotFound();
            }

            var categoriaExiste = await dataContext.Categorias
                .AnyAsync(c => c.Id == dto.CategoriaID);

            if (!categoriaExiste)
            {
                return BadRequest("La categoría indicada no existe.");
            }

            productoExistente.Nombre = dto.Nombre;
            productoExistente.PrecioCompra = dto.PrecioCompra;
            productoExistente.PrecioVenta = dto.PrecioVenta;
            productoExistente.Stock = dto.Stock;
            productoExistente.CategoriaId = dto.CategoriaID;

            await dataContext.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await dataContext.Productos
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound();
            }

            // Eliminar el archivo físico de la imagen, si existe
            if (producto.Imagen != null)
            {
                var rutaArchivo = Path.Combine(
                    _env.WebRootPath,
                    producto.Imagen.RutaRelativa
                );

                if (System.IO.File.Exists(rutaArchivo))
                {
                    System.IO.File.Delete(rutaArchivo);
                }
            }

            dataContext.Productos.Remove(producto);

            await dataContext.SaveChangesAsync();

            return NoContent();
        }
    }

}
