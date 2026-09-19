namespace TP5Programacion.Compartidas.DTO.Cliente.Request;

public record ActualizarClienteRequest(
    string Nombre,
    string Dni,
    string Telefono,
    string Email,
    string Direccion);