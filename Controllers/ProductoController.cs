using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TablexAPI.DTOs;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;
using TablexAPI.Services;

namespace TablexAPI.Controllers;

[ApiController]
[Route("api/productos")]
[Authorize]
[Produces("application/json")]
public class ProductoController : ControllerBase
{
    private readonly ProductoService _productoService;

    public ProductoController(ProductoService productoService)
    {
        _productoService = productoService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProductoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProductoDto>>> GetAll([FromQuery] string? type, CancellationToken ct)
    {
        return Ok(await _productoService.GetAllAsync(type, ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> GetById(int id, CancellationToken ct)
    {
        return Ok(await _productoService.GetByIdAsync(id, ct));
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpPost]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductoDto>> Create([FromBody] ProductoRequest request, CancellationToken ct)
    {
        var producto = await _productoService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductoDto>> Update(int id, [FromBody] ProductoRequest request, CancellationToken ct)
    {
        return Ok(await _productoService.UpdateAsync(id, request, ct));
    }

    [Authorize(Roles = Roles.Caja)]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _productoService.DeleteAsync(id, ct);
        return NoContent();
    }
}
