namespace API.DTOs
{
    public record ListaProductosDTO(
        int Id, 
        string Nombre, 
        decimal PrecioCompra, 
        decimal PrecioVenta, 
        int Stock, 
        int CategoriaID
        );

    public record CrearProductoDTO(
        string Nombre,
        decimal PrecioCompra,
        decimal PrecioVenta,
        int Stock,
        int CategoriaID
        );

    public record ProductoDetalleDTO(
        int Id,
        string Nombre,
        decimal PrecioCompra,
        decimal PrecioVenta,
        int Stock,
        int CategoriaId,
        string? CategoriaNombre,
        ImagenResponseDTO? Imagen
        );
    public record ImagenResponseDTO(
        int Id,
        string NombreOriginal,
        string NombreArchivo,
        string RutaRelativa,
        string TipoContenido,
        long TamanoBytes,
        DateTime FechaCreacion);

}
