namespace API.DTOs.Output
{
    public record ClienteDtoOutput(
        int Id,
        string Nombre,
        string Dni,
        string Telefono,
        string Email,
        string Direccion);
}
