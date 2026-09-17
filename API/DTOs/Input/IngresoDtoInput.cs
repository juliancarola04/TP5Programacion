namespace API.DTOs.Input
{
    public record ItemIngresoDtoInput(int ProductoId, int Cantidad);
    public record CrearIngresoDtoInput(
        int IdProovedor, 
        List<ItemIngresoDtoInput> Items);
}
