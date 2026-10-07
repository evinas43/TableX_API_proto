using System.ComponentModel.DataAnnotations;
using TablexAPI.Lib.Consts;

namespace TablexAPI.DTOs;

public class ProductoRequest
{
    [Required, StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required, AllowedValues(TiposProducto.Comestible, TiposProducto.Bebida, ErrorMessage = "El tipo debe ser 'comestible' o 'bebida'.")]
    public string Type { get; set; } = string.Empty;

    [Required]
    [Range(typeof(decimal), "0", "99999999.99",
        ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true,
        ErrorMessage = "El precio debe estar entre 0 y 99999999.99.")]
    public decimal? Precio { get; set; }
}
