using System.ComponentModel.DataAnnotations;

namespace TablexAPI.DTOs;

public class LoginRequest
{
    [Required, StringLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
