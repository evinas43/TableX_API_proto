namespace TablexAPI.DTOs;

public class PagoDto
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public decimal Importe { get; set; }
    public DateTime Fecha { get; set; }
    public string Metodo { get; set; } = string.Empty;
}
