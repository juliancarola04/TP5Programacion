namespace API.DTOs.Output
{
    public record ProductoListadoDtoOutput(
        int Id,
        string Nombre,
        decimal PrecioCompra,
        decimal PrecioVenta,
        int Stock,
        int CategoriaId);

    public record ProductoDetalleDtoOutput(
        int Id,
        string Nombre,
        decimal PrecioCompra,
        decimal PrecioVenta,
        int Stock,
        int CategoriaId,
        string? CategoriaNombre,
        ImagenDtoOutput? Imagen);

    public record ImagenDtoOutput(
    int Id,
    string NombreOriginal,
    string Url,
    string TipoContenido,
    long TamanoBytes,
    DateTime FechaCreacion);
}
