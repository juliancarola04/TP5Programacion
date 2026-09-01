namespace API.Models
{
    public class Ingreso
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public ICollection<DetalleIngreso> DetallesIngresos { get; set; } = new List<DetalleIngreso>();
    }
}
