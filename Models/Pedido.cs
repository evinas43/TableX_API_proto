namespace TablexAPI.Models;

public class Pedido
{
    public int Id { get; set; }
    public int MesaId { get; set; }
    public int UserId { get; set; }
    public DateTime Fecha { get; set; }
    public string Estado { get; set; } = null!;

    public Mesa Mesa { get; set; } = null!;
    public User User { get; set; } = null!;
    public ICollection<DetallePedido> Detalles { get; set; } = new List<DetallePedido>();
    public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
