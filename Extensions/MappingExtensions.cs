using TablexAPI.DTOs;
using TablexAPI.Models;

namespace TablexAPI.Extensions;

public static class MappingExtensions
{
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        Username = user.Username,
        Role = user.Role
    };

    public static ProductoDto ToDto(this Producto producto) => new()
    {
        Id = producto.Id,
        Name = producto.Name,
        Type = producto.Type,
        Precio = producto.Precio
    };

    public static PagoDto ToDto(this Pago pago) => new()
    {
        Id = pago.Id,
        PedidoId = pago.PedidoId,
        Importe = pago.Importe,
        Fecha = pago.Fecha,
        Metodo = pago.Metodo
    };

    public static decimal Subtotal(this DetallePedido detalle) => detalle.Precio * detalle.Cantidad;

    public static DetallePedidoDto ToDto(this DetallePedido detalle) => new()
    {
        Id = detalle.Id,
        ProductoId = detalle.ProductoId,
        ProductoName = detalle.Producto.Name,
        ProductoType = detalle.Producto.Type,
        Cantidad = detalle.Cantidad,
        Precio = detalle.Precio,
        Subtotal = detalle.Subtotal(),
        Pagado = detalle.Pagado
    };

    public static PedidoDto ToDto(this Pedido pedido)
    {
        var detalles = pedido.Detalles.OrderBy(d => d.Id).ToList();

        return new PedidoDto
        {
            Id = pedido.Id,
            MesaId = pedido.MesaId,
            MesaNumero = pedido.Mesa.Numero,
            UserId = pedido.UserId,
            Username = pedido.User.Username,
            Fecha = pedido.Fecha,
            Estado = pedido.Estado,
            Detalles = detalles.Select(d => d.ToDto()).ToList(),
            Total = detalles.Sum(d => d.Subtotal()),
            TotalPagado = pedido.Pagos.Sum(p => p.Importe),
            Pendiente = detalles.Where(d => !d.Pagado).Sum(d => d.Subtotal())
        };
    }
}
