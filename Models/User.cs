namespace TablexAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string Role { get; set; } = null!;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
