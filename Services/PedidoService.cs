using System.Data;
using Microsoft.EntityFrameworkCore;
using TablexAPI.DTOs;
using TablexAPI.Data;
using TablexAPI.Extensions;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;

namespace TablexAPI.Services;

public class PedidoService
{
    private readonly AppDbContext _db;

    public PedidoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PedidoDto> GetByIdAsync(int pedidoId, CancellationToken ct = default)
    {
        var pedido = await QueryWithDetails().FirstOrDefaultAsync(p => p.Id == pedidoId, ct)
            ?? throw new NotFoundException(ErrorMessages.PedidoNotFound);

        return pedido.ToDto();
    }

    public async Task<PedidoDto> GetPedidoAbiertoDeMesaAsync(int mesaId, CancellationToken ct = default)
    {
        if (!await _db.Mesas.AnyAsync(m => m.Id == mesaId, ct))
            throw new NotFoundException(ErrorMessages.MesaNotFound);

        var pedido = await QueryWithDetails()
            .Where(p => p.MesaId == mesaId && p.Estado == EstadosPedido.Abierto)
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(ErrorMessages.MesaWithoutOpenPedido);

        return pedido.ToDto();
    }

    public async Task<PedidoDto> AbrirPedidoAsync(int mesaId, int userId, CancellationToken ct = default)
    {
        var pedidoId = await _db.ExecuteInTransactionAsync(async token =>
        {
            if (!await _db.Mesas.AnyAsync(m => m.Id == mesaId, token))
                throw new NotFoundException(ErrorMessages.MesaNotFound);

            if (await _db.Pedidos.AnyAsync(p => p.MesaId == mesaId && p.Estado == EstadosPedido.Abierto, token))
                throw new ConflictException(ErrorMessages.MesaHasOpenPedido);

            var pedido = new Pedido
            {
                MesaId = mesaId,
                UserId = userId,
                Estado = EstadosPedido.Abierto
            };

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync(token);
            return pedido.Id;
        }, IsolationLevel.Serializable, ct);

        return await GetByIdAsync(pedidoId, ct);
    }

    public async Task<PedidoDto> CerrarPedidoAsync(int pedidoId, CancellationToken ct = default)
    {
        await _db.ExecuteInTransactionAsync(async token =>
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, token)
                ?? throw new NotFoundException(ErrorMessages.PedidoNotFound);

            EnsureAbierto(pedido);

            var pendientes = pedido.Detalles.Where(d => !d.Pagado).ToList();

            if (pendientes.Any(d => d.Subtotal() > 0))
                throw new ConflictException(ErrorMessages.PedidoHasPendingPayments);

            foreach (var detalle in pendientes)
                detalle.Pagado = true;

            pedido.Estado = EstadosPedido.Cerrado;
            await _db.SaveChangesAsync(token);
            return true;
        }, IsolationLevel.Serializable, ct);

        return await GetByIdAsync(pedidoId, ct);
    }

    public async Task<PedidoDto> AddDetalleAsync(int pedidoId, AddDetalleRequest request, CancellationToken ct = default)
    {
        var productoId = request.ProductoId!.Value;
        var cantidad = request.Cantidad!.Value;

        await _db.ExecuteInTransactionAsync(async token =>
        {
            var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId, token)
                ?? throw new NotFoundException(ErrorMessages.PedidoNotFound);

            EnsureAbierto(pedido);

            var producto = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == productoId, token)
                ?? throw new NotFoundException(ErrorMessages.ProductoNotFound);

            var existente = await _db.DetallesPedido.FirstOrDefaultAsync(d =>
                d.PedidoId == pedidoId &&
                d.ProductoId == productoId &&
                d.Precio == producto.Precio &&
                !d.Pagado, token);

            if (existente is not null)
            {
                existente.Cantidad += cantidad;
            }
            else
            {
                _db.DetallesPedido.Add(new DetallePedido
                {
                    PedidoId = pedidoId,
                    ProductoId = productoId,
                    Cantidad = cantidad,
                    Precio = producto.Precio,
                    Pagado = false
                });
            }

            await _db.SaveChangesAsync(token);
            return true;
        }, IsolationLevel.Serializable, ct);

        return await GetByIdAsync(pedidoId, ct);
    }

    public async Task<PedidoDto> UpdateDetalleAsync(int pedidoId, int detalleId, UpdateDetalleRequest request, CancellationToken ct = default)
    {
        var cantidad = request.Cantidad!.Value;

        await _db.ExecuteInTransactionAsync(async token =>
        {
            var detalle = await LoadDetalleEditableAsync(pedidoId, detalleId, token);
            detalle.Cantidad = cantidad;
            await _db.SaveChangesAsync(token);
            return true;
        }, IsolationLevel.Serializable, ct);

        return await GetByIdAsync(pedidoId, ct);
    }

    public async Task<PedidoDto> RemoveDetalleAsync(int pedidoId, int detalleId, CancellationToken ct = default)
    {
        await _db.ExecuteInTransactionAsync(async token =>
        {
            var detalle = await LoadDetalleEditableAsync(pedidoId, detalleId, token);
            _db.DetallesPedido.Remove(detalle);
            await _db.SaveChangesAsync(token);
            return true;
        }, IsolationLevel.Serializable, ct);

        return await GetByIdAsync(pedidoId, ct);
    }

    private IQueryable<Pedido> QueryWithDetails() =>
        _db.Pedidos.AsNoTracking()
            .Include(p => p.Mesa)
            .Include(p => p.User)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .Include(p => p.Pagos)
            .AsSplitQuery();

    private async Task<DetallePedido> LoadDetalleEditableAsync(int pedidoId, int detalleId, CancellationToken ct)
    {
        var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == pedidoId, ct)
            ?? throw new NotFoundException(ErrorMessages.PedidoNotFound);

        EnsureAbierto(pedido);

        var detalle = await _db.DetallesPedido.FirstOrDefaultAsync(d => d.Id == detalleId && d.PedidoId == pedidoId, ct)
            ?? throw new NotFoundException(ErrorMessages.DetalleNotFound);

        if (detalle.Pagado)
            throw new ConflictException(ErrorMessages.DetalleAlreadyPaid);

        return detalle;
    }

    internal static void EnsureAbierto(Pedido pedido)
    {
        if (pedido.Estado != EstadosPedido.Abierto)
            throw new ConflictException(ErrorMessages.PedidoClosed);
    }
}
