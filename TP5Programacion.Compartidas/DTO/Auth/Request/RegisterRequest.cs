namespace TP5Programacion.Compartidas.DTO.Auth.Request;

public record RegisterRequest(
    string Username,
    string Password,
    string Email);