namespace API.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Dni { get; set; } 
        public required string Telefono { get; set; } 
        public required string Email { get; set; } 
        public required string Direccion { get; set; }
        public ICollection<Venta>? Ventas { get; set; }
    }
}
