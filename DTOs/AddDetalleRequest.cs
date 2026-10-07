using System.ComponentModel.DataAnnotations;

namespace TablexAPI.DTOs;

public class AddDetalleRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? ProductoId { get; set; }

    [Required, Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000.")]
    public int? Cantidad { get; set; }
}
