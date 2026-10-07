namespace TablexAPI.DTOs;

public class DetallePedidoDto
{
    public int Id { get; set; }
    public int ProductoId { get; set; }
    public string ProductoName { get; set; } = string.Empty;
    public string ProductoType { get; set; } = string.Empty;
    public int Cantidad { get; set; }

    public decimal Precio { get; set; }

    public decimal Subtotal { get; set; }
    public bool Pagado { get; set; }
}
