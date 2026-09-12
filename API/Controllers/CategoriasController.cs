using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly CategoriaService _categoriaService;

    public CategoriasController(CategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoriaDtoOutput>>> ObtenerTodas()
    {
        try
        {
            return Ok(await _categoriaService.ObtenerTodas());
        }
        catch (BaseDeDatosException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDtoOutput>> ObtenerPorId(int id)
    {
        try
        {
            return Ok(await _categoriaService.ObtenerPorId(id));
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
    public async Task<ActionResult<CategoriaDtoOutput>> Crear(CrearCategoriaDtoInput dto)
    {
        try
        {
            CategoriaDtoOutput categoria = await _categoriaService.Crear(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.Id }, categoria);
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
    public async Task<IActionResult> Actualizar(int id, ActualizarCategoriaDtoInput dto)
    {
        try
        {
            await _categoriaService.Actualizar(id, dto);
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
            await _categoriaService.Eliminar(id);
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