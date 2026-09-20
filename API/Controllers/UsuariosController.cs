using API.Excepciones;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using API.Models.ModeloAuxiliar;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Usuario;
using TP5Programacion.Compartidas.DTO.Paginado.Response;
using TP5Programacion.Compartidas.DTO.Usuario.Request;
using TP5Programacion.Compartidas.DTO.Usuario.Response;

namespace API.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        // [Authorize(Roles = "Administrador")] Esto para mí debería estar descomentado, pero lo dejo así por ahora así no es tan paja usarlo.
        // {{baseURL}}/api/usuarios/?esadministrador=true ejemplo de API request para obtener solo a los administradores
        public async Task<ActionResult<PaginadoResponseDto<ObtenerUsuarioResponse>>> ObtenerTodos(
            [FromQuery] ParametroPaginacionUsuarioRequest parametros)
        {
            try
            {
                PaginadoResponse<ObtenerUsuarioResponse> usuarios = await _usuarioService.ObtenerlosATodos(parametros);
                
                PaginadoResponseDto<ObtenerUsuarioResponse> paginadoResponseDto = new PaginadoResponseDto<ObtenerUsuarioResponse>(usuarios.NumeroPagina,
                    usuarios.TamanoPagina, usuarios.TotalRegistros, usuarios.TotalPaginas, usuarios.TienePaginaAnterior,
                    usuarios.TienePaginaPosterior, usuarios.Datos);
                
                return Ok(paginadoResponseDto);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPost("convertirenadministrador/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> ConvertirEnAdministrador(int id)
        {
            try
            {
                await _usuarioService.ConvertirEnAdministrador(id);
                return Ok();
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

        [HttpPost("quitaradministrador/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> QuitarAdministrador(int id)
        {
            try
            {
                await _usuarioService.QuitarAdministrador(id);
                return Ok();
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

        [HttpDelete("dardebaja")]
        public async Task<ActionResult> DarDeBaja()
        {
            try
            {
                string? idClaimUsuario = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!int.TryParse(idClaimUsuario, out int id))
                {
                    return Unauthorized("No podés dar de baja a este usuario.");
                }

                await _usuarioService.DarDeBaja(id);
                return Ok();
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

        [HttpPut("actualizar")]
        public async Task<ActionResult<ActualizarUsuarioResponse>> Actualizar([FromBody] ActualizarUsuarioRequest actualizarUsuarioRequest)
        {
            try
            {
                string? idClaimUsuario = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!int.TryParse(idClaimUsuario, out int id))
                {
                    return Unauthorized("No podés actualizar a este usuario.");
                }

                ActualizarUsuarioResponse actualizarUsuarioResponse = await _usuarioService.Actualizar(id, actualizarUsuarioRequest);
                return Ok(actualizarUsuarioResponse);
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
        public async Task<ActionResult> DarDeBajaAdmin(int id)
        {
            try
            {
                await _usuarioService.DarDeBaja(id);
                return Ok();
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

        [HttpPut("admin/actualizar/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> ActualizarAdmin(int id, [FromBody] ActualizarUsuarioRequest actualizarUsuarioRequest)
        {
            try
            {
                await _usuarioService.Actualizar(id, actualizarUsuarioRequest);
                return Ok();
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

    }
}
