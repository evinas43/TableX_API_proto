using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TablexAPI.DTOs;
using TablexAPI.Extensions;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;
using TablexAPI.Services;

namespace TablexAPI.Controllers;

[ApiController]
[Route("api/mesas")]
[Authorize]
[Produces("application/json")]
public class MesaController : ControllerBase
{
    private readonly MesaService _mesaService;
    private readonly PedidoService _pedidoService;

    public MesaController(MesaService mesaService, PedidoService pedidoService)
    {
        _mesaService = mesaService;
        _pedidoService = pedidoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MesaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MesaDto>>> GetAll(CancellationToken ct)
    {
        return Ok(await _mesaService.GetAllAsync(ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MesaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MesaDto>> GetById(int id, CancellationToken ct)
    {
        return Ok(await _mesaService.GetByIdAsync(id, ct));
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpPost]
    [ProducesResponseType(typeof(MesaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MesaDto>> Create([FromBody] MesaRequest request, CancellationToken ct)
    {
        var mesa = await _mesaService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = mesa.Id }, mesa);
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MesaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MesaDto>> Update(int id, [FromBody] MesaRequest request, CancellationToken ct)
    {
        return Ok(await _mesaService.UpdateAsync(id, request, ct));
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _mesaService.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{mesaId:int}/pedido-abierto")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoDto>> GetPedidoAbierto(int mesaId, CancellationToken ct)
    {
        return Ok(await _pedidoService.GetPedidoAbiertoDeMesaAsync(mesaId, ct));
    }

    [Authorize(Roles = Roles.Camarero)]
    [HttpPost("{mesaId:int}/pedidos")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PedidoDto>> AbrirPedido(int mesaId, CancellationToken ct)
    {
        var pedido = await _pedidoService.AbrirPedidoAsync(mesaId, User.GetUserId(), ct);
        return CreatedAtAction(nameof(PedidoController.GetById), "Pedido", new { id = pedido.Id }, pedido);
    }
}
