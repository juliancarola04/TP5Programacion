namespace API.DTOs.Input
{
    public record ItemIngresoDtoInput(int ProductoId, int Cantidad, decimal PrecioUnitario);

    public record CrearIngresoDtoInput(
        int ProveedorId,
        List<ItemIngresoDtoInput> Items);
}
