namespace API.DTOs.Input
{
    public record CrearClienteDtoInput(
        string Nombre,
        string Dni,
        string Telefono,
        string Email,
        string Direccion);

    public record ActualizarClienteDtoInput(
        string Nombre,
        string Dni,
        string Telefono,
        string Email,
        string Direccion);
}
