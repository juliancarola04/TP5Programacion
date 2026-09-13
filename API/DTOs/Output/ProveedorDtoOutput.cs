namespace API.DTOs.Output;

public class ProveedorDtoOutput
{
    public required int Id { get; set; }
    public required string RazonSocial { get; set; }
    public required string Cuit { get; set; }
    public required string Direccion { get; set; }
    public required string Email { get; set; }
    public required string Telefono { get; set; }
}