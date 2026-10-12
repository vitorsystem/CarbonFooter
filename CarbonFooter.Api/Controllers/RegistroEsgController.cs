using CarbonFooter.Api.Models;
using CarbonFooter.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarbonFooter.Api.Controllers;

[ApiController]
[Route("api/esg")]
public class RegistroEsgController : ControllerBase
{
    private readonly RegistroEsgService _service;

    public RegistroEsgController(RegistroEsgService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<RegistroEsg>>> Listar()
    {
        var registros = await _service.ListarTodosAsync();
        return Ok(registros);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegistroEsg>> BuscarPorId(string id)
    {
        var registro = await _service.BuscarPorIdAsync(id);
        if (registro == null) return NotFound();
        return Ok(registro);
    }

    [HttpPost]
    public async Task<ActionResult<RegistroEsg>> Criar(RegistroEsg registro)
    {
        var salvo = await _service.SalvarAsync(registro);
        return CreatedAtAction(nameof(BuscarPorId), new { id = salvo.Id }, salvo);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Deletar(string id)
    {
        await _service.DeletarAsync(id);
        return NoContent();
    }
}