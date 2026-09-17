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
        int ProovedorId,
        string RazonSocial,
        int UsuarioId,
        string UsuarioUsername,
        List<DetalleIngresoDtoOutput> Detalles);

    public record IngresoListadoDtoOutput(
        int Id,
        DateTime Fecha,
        decimal Total,
        int ProveedorId,
        string ProovedorRazonSocial);
}

