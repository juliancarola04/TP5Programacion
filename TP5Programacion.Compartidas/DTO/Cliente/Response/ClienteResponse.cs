namespace TP5Programacion.Compartidas.DTO.Cliente.Response;

public record ClienteResponse(
    int Id,
    string Nombre,
    string Dni,
    string Telefono,
    string Email,
    string Direccion);