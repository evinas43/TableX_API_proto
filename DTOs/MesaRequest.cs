using System.ComponentModel.DataAnnotations;

namespace TablexAPI.DTOs;

public class MesaRequest
{
    [Required, Range(1, int.MaxValue, ErrorMessage = "El número de mesa debe ser mayor que 0.")]
    public int? Numero { get; set; }
}
