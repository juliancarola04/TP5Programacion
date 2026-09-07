namespace API.DTOs.Input
{
    public record CrearProductoDtoInput(
        string Nombre,
        decimal PrecioCompra,
        decimal PrecioVenta,
        int Stock,
        int CategoriaId);

    public record ActualizarProductoDtoInput(
        string Nombre,
        decimal PrecioCompra,
        decimal PrecioVenta,
        int Stock,
        int CategoriaId);
}
