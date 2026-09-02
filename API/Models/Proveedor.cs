namespace API.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public required string RazonSocial { get; set; }
        public required string Telefono { get; set; } 
        public required string Email { get; set; } 
        public required string Direccion { get; set; } 
        public ICollection<Ingreso>? Ingresos { get; set; }
    }
}
