namespace TP5Programacion.Compartidas.DTO.Usuario.Request;

public record ActualizarUsuarioRequest(
    string? Username,
    string? Password,
    string? Email);