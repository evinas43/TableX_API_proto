namespace TablexAPI.Models;

public class Pago
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public decimal Importe { get; set; }
    public DateTime Fecha { get; set; }
    public string Metodo { get; set; } = null!;

    public Pedido Pedido { get; set; } = null!;
}
