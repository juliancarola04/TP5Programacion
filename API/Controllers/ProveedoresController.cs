using API.Excepciones;
using API.Models.ModeloAuxiliar;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Usuario;
using TP5Programacion.Compartidas.DTO.Paginado.Response;
using TP5Programacion.Compartidas.DTO.Proveedor.Request;
using TP5Programacion.Compartidas.DTO.Proveedor.Response;
using TP5Programacion.Compartidas.DTO.Usuario.Response;

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
        public async Task<ActionResult<PaginadoResponseDto<ObtenerProveedorResponse>>> ObtenerTodos(
                [FromQuery] ParametroPaginacionProveedorRequest parametros)
        {
            try
            {
                PaginadoResponse<ObtenerProveedorResponse> proveedores = await _proveedorService.ObtenerTodos(parametros);
                
                PaginadoResponseDto<ObtenerProveedorResponse> paginadoResponseDto = new PaginadoResponseDto<ObtenerProveedorResponse>(proveedores.NumeroPagina,
                    proveedores.TamanoPagina, proveedores.TotalRegistros, proveedores.TotalPaginas, proveedores.TienePaginaAnterior,
                    proveedores.TienePaginaPosterior, proveedores.Datos);
                
                return Ok(paginadoResponseDto);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("admin/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ObtenerProveedorResponse>> ObtenerPorId(int id)
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
        public async Task<ActionResult<CrearProveedorResponse>> Crear(CrearProveedorRequest dto)
        {
            try
            {
                CrearProveedorResponse proveedorDtoOutput = await _proveedorService.Crear(dto);
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
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Actualizar(int id, ActualizarProveedorRequest dto)
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
        [Authorize(Roles = "Administrador")]
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
