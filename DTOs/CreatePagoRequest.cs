using System.ComponentModel.DataAnnotations;
using TablexAPI.Lib.Consts;

namespace TablexAPI.DTOs;

public class CreatePagoRequest
{
    [Required, AllowedValues(MetodosPago.Efectivo, MetodosPago.Tarjeta, ErrorMessage = "El método debe ser 'efectivo' o 'tarjeta'.")]
    public string Metodo { get; set; } = string.Empty;

    public List<int>? DetalleIds { get; set; }
}
