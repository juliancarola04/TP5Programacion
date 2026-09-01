namespace API.Models
{
    public class DetalleVenta
    {
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
        public int VentaId { get; set; }
        public Venta Venta { get; set; } = null!;
    }
}
