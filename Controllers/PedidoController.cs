using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TablexAPI.DTOs;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;
using TablexAPI.Services;

namespace TablexAPI.Controllers;

[ApiController]
[Route("api/pedidos")]
[Authorize]
[Produces("application/json")]
public class PedidoController : ControllerBase
{
    private readonly PedidoService _pedidoService;
    private readonly PagoService _pagoService;

    public PedidoController(PedidoService pedidoService, PagoService pagoService)
    {
        _pedidoService = pedidoService;
        _pagoService = pagoService;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PedidoDto>> GetById(int id, CancellationToken ct)
    {
        return Ok(await _pedidoService.GetByIdAsync(id, ct));
    }

    [Authorize(Roles = Roles.Camarero)]
    [HttpPost("{id:int}/detalles")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PedidoDto>> AddDetalle(int id, [FromBody] AddDetalleRequest request, CancellationToken ct)
    {
        return Ok(await _pedidoService.AddDetalleAsync(id, request, ct));
    }

    [Authorize(Roles = Roles.Camarero)]
    [HttpPut("{id:int}/detalles/{detalleId:int}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PedidoDto>> UpdateDetalle(int id, int detalleId, [FromBody] UpdateDetalleRequest request, CancellationToken ct)
    {
        return Ok(await _pedidoService.UpdateDetalleAsync(id, detalleId, request, ct));
    }

    [Authorize(Roles = Roles.Camarero)]
    [HttpDelete("{id:int}/detalles/{detalleId:int}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PedidoDto>> RemoveDetalle(int id, int detalleId, CancellationToken ct)
    {
        return Ok(await _pedidoService.RemoveDetalleAsync(id, detalleId, ct));
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpPost("{id:int}/cerrar")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PedidoDto>> Cerrar(int id, CancellationToken ct)
    {
        return Ok(await _pedidoService.CerrarPedidoAsync(id, ct));
    }

    [HttpGet("{id:int}/pagos")]
    [ProducesResponseType(typeof(List<PagoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PagoDto>>> GetPagos(int id, CancellationToken ct)
    {
        return Ok(await _pagoService.GetPagosDePedidoAsync(id, ct));
    }

    [HttpGet("{id:int}/pendiente")]
    [ProducesResponseType(typeof(PendienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PendienteDto>> GetPendiente(int id, CancellationToken ct)
    {
        return Ok(await _pagoService.GetPendienteAsync(id, ct));
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpPost("{id:int}/pagos")]
    [ProducesResponseType(typeof(PagoRegistradoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PagoRegistradoDto>> RegistrarPago(int id, [FromBody] CreatePagoRequest request, CancellationToken ct)
    {
        var result = await _pagoService.RegistrarPagoAsync(id, request, ct);
        return CreatedAtAction(nameof(GetPagos), new { id }, result);
    }
}
