namespace API.Models
{
    public class DetalleIngreso
    {
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int IngresoId { get; set; }
        public Ingreso Ingreso { get; set; } = null!;
    }
}
