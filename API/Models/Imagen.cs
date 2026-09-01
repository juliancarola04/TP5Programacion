namespace API.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public required string NombreOriginal { get; set; }
        public required string NombreArchivo { get; set; }
        public required string RutaRelativa { get; set; }
        public required string TipoContenido { get; set; }
        public long TamanoBytes { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;
    }
}
