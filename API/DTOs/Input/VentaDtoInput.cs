namespace API.DTOs.Input
{
    public record ItemVentaDtoInput(int ProductoId, int Cantidad);
    public record CrearVentaDtoInput(
        int ClienteId,
        List<ItemVentaDtoInput> Items);
}
