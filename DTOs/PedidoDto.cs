namespace TablexAPI.DTOs;

public class PedidoDto
{
    public int Id { get; set; }
    public int MesaId { get; set; }
    public int MesaNumero { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = string.Empty;

    public List<DetallePedidoDto> Detalles { get; set; } = new();

    public decimal Total { get; set; }

    public decimal TotalPagado { get; set; }

    public decimal Pendiente { get; set; }
}
