using System.Security.Claims;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VentasController : ControllerBase
{
    private readonly VentaService _ventaService;
    public VentasController(VentaService ventaService)
    {
        _ventaService = ventaService;
    }
    [HttpGet]
    public async Task<ActionResult<List<VentaListadoDtoOutput>>> ObtenerTodas()
    {
        try
        {
            return Ok(await _ventaService.ObtenerTodas());
        }
        catch (BaseDeDatosException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<VentaDtoOutput>> ObtenerPorId(int id)
    {
        try
        {
            return Ok(await _ventaService.ObtenerPorId(id));
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
    public async Task<ActionResult<VentaDtoOutput>> Crear(CrearVentaDtoInput dto)
    {
        string? usuarioIdClaim = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (usuarioIdClaim is null || !int.TryParse(usuarioIdClaim, out int usuarioId))
        {
            return Unauthorized("No se pudo identificar al usuario a partir del token.");
        }

        try
        {
            VentaDtoOutput venta = await _ventaService.Crear(dto, usuarioId);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = venta.Id }, venta);
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
    public async Task<IActionResult> Anular(int id)
    {
        try
        {
            await _ventaService.Anular(id);
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
}


