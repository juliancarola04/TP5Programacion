namespace TP5Programacion.Compartidas.DTO.Cliente.Request;

public record CrearClienteRequest(
    string Nombre,
    string Dni,
    string Telefono,
    string Email,
    string Direccion);