namespace TP5Programacion.Compartidas.DTO.Producto.Response;

public record ProductoListadoResponse(
    int Id,
    string Nombre,
    decimal PrecioCompra,
    decimal PrecioVenta,
    int Stock,
    int CategoriaId);
    
public record ProductoResponse(
    int Id,
    string Nombre,
    decimal PrecioCompra,
    decimal PrecioVenta,
    int Stock,
    int CategoriaId,
    string? CategoriaNombre,
    ProductoImagenResponse? Imagen);
    
public record ProductoImagenResponse(
    int Id,
    string NombreOriginal,
    string Url,
    string TipoContenido,
    long TamanoBytes,
    DateTime FechaCreacion);