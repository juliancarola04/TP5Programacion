using API.Excepciones;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5Programacion.Compartidas.DTO.Producto.Request;
using TP5Programacion.Compartidas.DTO.Producto.Response;

using API.Models.ModeloAuxiliar;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Producto;
using TP5Programacion.Compartidas.DTO.Paginado.Response;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductosController : ControllerBase
    {
        private readonly ProductoService _productoService;
        private readonly ImagenService _imagenService;

        // Actualizá el constructor para inyectarlo también:
        public ProductosController(ProductoService productoService, ImagenService imagenService)
        {
            _productoService = productoService;
            _imagenService = imagenService;
        }

        [HttpPost("{id}/imagen")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProductoImagenResponse>> SubirImagen(int id, IFormFile archivo)
        {
            try
            {
                Imagen imagen = await _imagenService.SubirImagen(id, archivo);

                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                ProductoImagenResponse dto = new ProductoImagenResponse(
                    imagen.Id,
                    imagen.NombreOriginal,
                    $"{baseUrl}/{imagen.RutaRelativa}",
                    imagen.TipoContenido,
                    imagen.TamanoBytes,
                    imagen.FechaCreacion);

                return CreatedAtAction(nameof(ObtenerPorId), new { id }, dto);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginadoResponseDto<ProductoListadoResponse>>> ObtenerTodos(
            [FromQuery] ParametroPaginacionProductoRequest parametros)
        {
            try
            {
                PaginadoResponse<ProductoListadoResponse> productos = await _productoService.ObtenerTodos(parametros);

                PaginadoResponseDto<ProductoListadoResponse> paginadoResponseDto = new PaginadoResponseDto<ProductoListadoResponse>(productos.NumeroPagina,
                    productos.TamanoPagina, productos.TotalRegistros, productos.TotalPaginas, productos.TienePaginaAnterior,
                    productos.TienePaginaPosterior, productos.Datos);

                return Ok(paginadoResponseDto);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoResponse>> ObtenerPorId(int id)
        {
            try
            {
                Producto producto = await _productoService.ObtenerPorId(id);

                string? imagenUrl = producto.Imagen is null
                    ? null
                    : $"{Request.Scheme}://{Request.Host}/{producto.Imagen.RutaRelativa}";

                ProductoImagenResponse? imagenDto = producto.Imagen is null
                    ? null
                    : new ProductoImagenResponse(
                        producto.Imagen.Id,
                        producto.Imagen.NombreOriginal,
                        imagenUrl!,
                        producto.Imagen.TipoContenido,
                        producto.Imagen.TamanoBytes,
                        producto.Imagen.FechaCreacion);

                ProductoResponse dto = new ProductoResponse(
                    producto.Id,
                    producto.Nombre,
                    producto.PrecioCompra,
                    producto.PrecioVenta,
                    producto.Stock,
                    producto.CategoriaId,
                    producto.Categoria?.Nombre,
                    imagenDto);

                return Ok(dto);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProductoListadoResponse>> Crear(CrearProductoRequest dto)
        {
            try
            {
                ProductoListadoResponse producto = await _productoService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(int id, ActualizarProductoRequest dto)
        {
            try
            {
                await _productoService.Actualizar(id, dto);
                return NoContent();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _productoService.Eliminar(id);
                return NoContent();
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

    }
}
