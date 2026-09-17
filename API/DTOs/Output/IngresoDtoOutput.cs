namespace API.DTOs.Output
{
    public record DetalleIngresoDtoOutput(
        int ProductoId,
        string ProductoNombre,
        int Cantidad,
        decimal PrecioUnitario,
        decimal Subtotal);

    public record IngresoDtoOutput(
        int Id,
        DateTime Fecha,
        decimal Total,
        int ProveedorId,
        string ProveedorRazonSocial,
        int UsuarioId,
        string UsuarioUsername,
        bool Anulado,
        List<DetalleIngresoDtoOutput> Detalles);

    public record IngresoListadoDtoOutput(
        int Id,
        DateTime Fecha,
        decimal Total,
        int ProveedorId,
        string ProveedorRazonSocial,
        bool Anulado);
}

