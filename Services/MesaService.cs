using Microsoft.EntityFrameworkCore;
using TablexAPI.DTOs;
using TablexAPI.Data;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;

namespace TablexAPI.Services;

public class MesaService
{
    private readonly AppDbContext _db;

    public MesaService(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<MesaDto>> GetAllAsync(CancellationToken ct = default)
    {
        return ProjectToDto(_db.Mesas.AsNoTracking())
            .OrderBy(m => m.Numero)
            .ToListAsync(ct);
    }

    public async Task<MesaDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await ProjectToDto(_db.Mesas.AsNoTracking().Where(m => m.Id == id))
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(ErrorMessages.MesaNotFound);
    }

    public async Task<MesaDto> CreateAsync(MesaRequest request, CancellationToken ct = default)
    {
        var numero = request.Numero!.Value;

        if (await _db.Mesas.AnyAsync(m => m.Numero == numero, ct))
            throw new ConflictException(ErrorMessages.MesaNumeroAlreadyExists);

        var mesa = new Mesa { Numero = numero };
        _db.Mesas.Add(mesa);
        await _db.SaveChangesAsync(ct);

        return new MesaDto { Id = mesa.Id, Numero = mesa.Numero, TienePedidoAbierto = false };
    }

    public async Task<MesaDto> UpdateAsync(int id, MesaRequest request, CancellationToken ct = default)
    {
        var numero = request.Numero!.Value;

        var mesa = await _db.Mesas.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException(ErrorMessages.MesaNotFound);

        if (await _db.Mesas.AnyAsync(m => m.Numero == numero && m.Id != id, ct))
            throw new ConflictException(ErrorMessages.MesaNumeroAlreadyExists);

        mesa.Numero = numero;
        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var mesa = await _db.Mesas.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException(ErrorMessages.MesaNotFound);

        if (await _db.Pedidos.AnyAsync(p => p.MesaId == id, ct))
            throw new ConflictException(ErrorMessages.MesaHasPedidos);

        _db.Mesas.Remove(mesa);
        await _db.SaveChangesAsync(ct);
    }

    private static IQueryable<MesaDto> ProjectToDto(IQueryable<Mesa> query) =>
        query.Select(m => new MesaDto
        {
            Id = m.Id,
            Numero = m.Numero,
            TienePedidoAbierto = m.Pedidos.Any(p => p.Estado == EstadosPedido.Abierto),
            PedidoAbiertoId = m.Pedidos
                .Where(p => p.Estado == EstadosPedido.Abierto)
                .OrderByDescending(p => p.Id)
                .Select(p => (int?)p.Id)
                .FirstOrDefault()
        });
}
