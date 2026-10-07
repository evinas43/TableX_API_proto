namespace TablexAPI.DTOs;

public class PendienteDto
{
    public int PedidoId { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPagado { get; set; }
    public decimal Pendiente { get; set; }
    public List<DetallePedidoDto> DetallesPendientes { get; set; } = new();
}
