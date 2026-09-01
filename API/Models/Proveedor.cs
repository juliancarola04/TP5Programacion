namespace API.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public required string RazonSocial { get; set; }
        public ICollection<Ingreso>? Ingresos { get; set; }
    }
}
