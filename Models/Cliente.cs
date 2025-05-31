using Distribuidora.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string Direccion { get; set; } = null!;
    public bool Estado { get; set; } = true;

    public int? UserEliminaId { get; set; }
    public User? UserElimina { get; set; }

    public DateTime? FechaDesactivacion { get; set; }

    public ICollection<ClientePedido> Pedidos { get; set; } = new List<ClientePedido>();
}