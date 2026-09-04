namespace API.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public ICollection<Venta>? Ventas { get; set; }
        public ICollection<Ingreso>? Ingresos { get; set; }
    }
}
