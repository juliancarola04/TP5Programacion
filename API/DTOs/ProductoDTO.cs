namespace API.DTOs
{
    public class ProductoDTO
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int CategoriaId { get; set; }

    }
}
