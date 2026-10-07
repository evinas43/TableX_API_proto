using System.Data;
using Microsoft.EntityFrameworkCore;
using TablexAPI.DTOs;
using TablexAPI.Data;
using TablexAPI.Extensions;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;

namespace TablexAPI.Services;

public class PagoService
{
    private readonly AppDbContext _db;
    private readonly PedidoService _pedidoService;

    public PagoService(AppDbContext db, PedidoService pedidoService)
    {
        _db = db;
        _pedidoService = pedidoService;
    }

    public async Task<List<PagoDto>> GetPagosDePedidoAsync(int pedidoId, CancellationToken ct = default)
    {
        await EnsurePedidoExistsAsync(pedidoId, ct);

        var pagos = await _db.Pagos.AsNoTracking()
            .Where(p => p.PedidoId == pedidoId)
            .OrderBy(p => p.Fecha).ThenBy(p => p.Id)
            .ToListAsync(ct);

        return pagos.Select(p => p.ToDto()).ToList();
    }

    public async Task<PendienteDto> GetPendienteAsync(int pedidoId, CancellationToken ct = default)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId, ct);

        return new PendienteDto
        {
            PedidoId = pedido.Id,
            Total = pedido.Total,
            TotalPagado = pedido.TotalPagado,
            Pendiente = pedido.Pendiente,
            DetallesPendientes = pedido.Detalles.Where(d => !d.Pagado).ToList()
        };
    }

    public async Task<PagoRegistradoDto> RegistrarPagoAsync(int pedidoId, CreatePagoRequest request, CancellationToken ct = default)
    {
        var detalleIdsSolicitados = request.DetalleIds?.Distinct().ToList() ?? new List<int>();

        var (pago, detallesPagados) = await _db.ExecuteInTransactionAsync(async token =>
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, token)
                ?? throw new NotFoundException(ErrorMessages.PedidoNotFound);

            PedidoService.EnsureAbierto(pedido);

            List<DetallePedido> aPagar;

            if (detalleIdsSolicitados.Count == 0)
            {
                aPagar = pedido.Detalles.Where(d => !d.Pagado).ToList();
            }
            else
            {
                aPagar = pedido.Detalles.Where(d => detalleIdsSolicitados.Contains(d.Id)).ToList();

                if (aPagar.Count != detalleIdsSolicitados.Count)
                    throw new BadRequestException(ErrorMessages.DetallesNotInPedido);

                if (aPagar.Any(d => d.Pagado))
                    throw new ConflictException(ErrorMessages.DetallesAlreadyPaid);
            }

            var importe = aPagar.Sum(d => d.Subtotal());

            if (importe <= 0)
                throw new ConflictException(ErrorMessages.NothingToPay);

            var nuevoPago = new Pago
            {
                PedidoId = pedidoId,
                Importe = importe,
                Metodo = request.Metodo
            };
            _db.Pagos.Add(nuevoPago);

            foreach (var detalle in aPagar)
                detalle.Pagado = true;

            await _db.SaveChangesAsync(token);

            return (nuevoPago.ToDto(), aPagar.Select(d => d.Id).OrderBy(id => id).ToList());
        }, IsolationLevel.Serializable, ct);

        return new PagoRegistradoDto
        {
            Pago = pago,
            DetallesPagados = detallesPagados,
            Pedido = await _pedidoService.GetByIdAsync(pedidoId, ct)
        };
    }

    private async Task EnsurePedidoExistsAsync(int pedidoId, CancellationToken ct)
    {
        if (!await _db.Pedidos.AnyAsync(p => p.Id == pedidoId, ct))
            throw new NotFoundException(ErrorMessages.PedidoNotFound);
    }
}
