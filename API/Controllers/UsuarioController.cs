using API.Data;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        // [Authorize(Roles = "Administrador")] Esto para mí debería estar descomentado, pero lo dejo así por ahora así no es tan paja usarlo.
        public async Task<ActionResult<List<UsuarioDtoOutput>>> ObtenerTodos()
        {
            try
            {
                List<UsuarioDtoOutput> usuarios = await _usuarioService.ObtenerlosATodos();
                return Ok(usuarios);
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

        [HttpPost("dardebaja")]
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

        [HttpPost("actualizar")]
        public async Task<ActionResult<UsuarioAuthDtoOutput>> Actualizar([FromBody] UsuarioDtoInput usuarioDtoInput)
        {
            try
            {
                string? idClaimUsuario = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!int.TryParse(idClaimUsuario, out int id))
                {
                    return Unauthorized("No podés actualizar a este usuario.");
                }

                UsuarioAuthDtoOutput usuarioAuthDtoOutput = await _usuarioService.Actualizar(id, usuarioDtoInput);
                return Ok(usuarioAuthDtoOutput);
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

        [HttpPost("admin/dardebaja/{id:int}")]
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

        [HttpPost("admin/actualizar/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> ActualizarAdmin(int id, [FromBody] UsuarioDtoInput usuarioDtoInput)
        {
            try
            {
                await _usuarioService.Actualizar(id, usuarioDtoInput);
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
