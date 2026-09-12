using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/proveedores")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly ProveedorService _proveedorService;

        public ProveedoresController(ProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<List<ProveedorDtoOutput>>> ObtenerTodos()
        {
            try
            {
                return Ok(await _proveedorService.ObtenerTodos());
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("admin/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProveedorDtoOutput>> ObtenerPorId(int id)
        {
            try
            {
                return Ok(await _proveedorService.ObtenerPorId(id));
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

        [HttpPost("admin/crear")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ProveedorDtoOutput>> Crear(ProveedorDtoInput dto)
        {
            try
            {
                ProveedorDtoOutput proveedorDtoOutput = await _proveedorService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = proveedorDtoOutput.Id }, proveedorDtoOutput);
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

        [HttpPut("admin/actualizar/{id:int}")]
        public async Task<IActionResult> Actualizar(int id, ProveedorDtoInput dto)
        {
            try
            {
                await _proveedorService.Actualizar(id, dto);
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
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("admin/dardebaja/{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _proveedorService.DarDeBaja(id);
                return NoContent();
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(e.Message);
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
    }
}
