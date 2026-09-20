namespace TP5Programacion.Compartidas.DTO.Paginado.Request.Usuario;

public record ParametroPaginacionUsuarioRequest : ParametroPaginacionRequest
{
    public bool? Eliminado { get; init; }
    public bool? EsAdministrador { get; init; }
};