namespace TablexAPI.DTOs;

public class PagoRegistradoDto
{
    public PagoDto Pago { get; set; } = null!;
    public List<int> DetallesPagados { get; set; } = new();
    public PedidoDto Pedido { get; set; } = null!;
}
