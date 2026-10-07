using Microsoft.EntityFrameworkCore;
using TablexAPI.DTOs;
using TablexAPI.Data;
using TablexAPI.Extensions;
using TablexAPI.Lib.Consts;
using TablexAPI.Models;

namespace TablexAPI.Services;

public class ProductoService
{
    private readonly AppDbContext _db;

    public ProductoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductoDto>> GetAllAsync(string? type, CancellationToken ct = default)
    {
        var query = _db.Productos.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(type))
        {
            if (!TiposProducto.All.Contains(type))
                throw new BadRequestException(ErrorMessages.InvalidProductoType);

            query = query.Where(p => p.Type == type);
        }

        var productos = await query.OrderBy(p => p.Type).ThenBy(p => p.Name).ToListAsync(ct);
        return productos.Select(p => p.ToDto()).ToList();
    }

    public async Task<ProductoDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var producto = await _db.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException(ErrorMessages.ProductoNotFound);
        return producto.ToDto();
    }

    public async Task<ProductoDto> CreateAsync(ProductoRequest request, CancellationToken ct = default)
    {
        var producto = new Producto
        {
            Name = request.Name.Trim(),
            Type = request.Type,
            Precio = request.Precio!.Value
        };

        _db.Productos.Add(producto);
        await _db.SaveChangesAsync(ct);

        return producto.ToDto();
    }

    public async Task<ProductoDto> UpdateAsync(int id, ProductoRequest request, CancellationToken ct = default)
    {
        var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException(ErrorMessages.ProductoNotFound);

        producto.Name = request.Name.Trim();
        producto.Type = request.Type;
        producto.Precio = request.Precio!.Value;

        await _db.SaveChangesAsync(ct);
        return producto.ToDto();
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var producto = await _db.Productos.FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException(ErrorMessages.ProductoNotFound);

        if (await _db.DetallesPedido.AnyAsync(d => d.ProductoId == id, ct))
            throw new ConflictException(ErrorMessages.ProductoInUse);

        _db.Productos.Remove(producto);
        await _db.SaveChangesAsync(ct);
    }
}
