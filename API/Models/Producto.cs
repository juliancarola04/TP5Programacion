namespace API.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; } = null!;
        public ICollection<DetalleIngreso> DetallesIngresos { get; set; } = new List<DetalleIngreso>();
        public ICollection<DetalleVenta> DetallesVentas { get; set; } = new List<DetalleVenta>();
        public Imagen? Imagen { get; set; }
    }
}
