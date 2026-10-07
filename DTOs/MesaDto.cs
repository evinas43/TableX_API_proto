namespace TablexAPI.DTOs;

public class MesaDto
{
    public int Id { get; set; }
    public int Numero { get; set; }
    public bool TienePedidoAbierto { get; set; }
    public int? PedidoAbiertoId { get; set; }
}
