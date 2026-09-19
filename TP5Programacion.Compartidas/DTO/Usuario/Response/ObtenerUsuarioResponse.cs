namespace TP5Programacion.Compartidas.DTO.Usuario.Response;

public record ObtenerUsuarioResponse(
    int Id,
    string Username,
    string Email,
    bool EsAdministrador);