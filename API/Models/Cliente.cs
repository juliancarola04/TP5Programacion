namespace API.Models
{
    public class Cliente
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public ICollection<Venta>? Ventas { get; set; }
    }
}
