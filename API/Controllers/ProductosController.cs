using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ImagenDtoOutput>> SubirImagen(int id, IFormFile archivo)
        {
            try
            {
                Imagen imagen = await _imagenService.SubirImagen(id, archivo);

                string baseUrl = $"{Request.Scheme}://{Request.Host}";
                ImagenDtoOutput dto = new ImagenDtoOutput(
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
        public async Task<ActionResult<List<ProductoListadoDtoOutput>>> ObtenerTodos()
        {
            try
            {
                return Ok(await _productoService.ObtenerTodos());
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDetalleDtoOutput>> ObtenerPorId(int id)
        {
            try
            {
                Producto producto = await _productoService.ObtenerPorId(id);

                string? imagenUrl = producto.Imagen is null
                    ? null
                    : $"{Request.Scheme}://{Request.Host}/{producto.Imagen.RutaRelativa}";

                ImagenDtoOutput? imagenDto = producto.Imagen is null
                    ? null
                    : new ImagenDtoOutput(
                        producto.Imagen.Id,
                        producto.Imagen.NombreOriginal,
                        imagenUrl!,
                        producto.Imagen.TipoContenido,
                        producto.Imagen.TamanoBytes,
                        producto.Imagen.FechaCreacion);

                ProductoDetalleDtoOutput dto = new ProductoDetalleDtoOutput(
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
        public async Task<ActionResult<ProductoListadoDtoOutput>> Crear(CrearProductoDtoInput dto)
        {
            try
            {
                ProductoListadoDtoOutput producto = await _productoService.Crear(dto);
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
        public async Task<IActionResult> Actualizar(int id, ActualizarProductoDtoInput dto)
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
