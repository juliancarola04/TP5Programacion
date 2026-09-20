using API.Models.ModeloAuxiliar.Query;

namespace API.Models.ModeloAuxiliar.UsuarioQuery;

public record UsuarioQueryParametros : QueryParametros
{
    public bool? Eliminado { get; init; }
    public bool? EsAdministrador { get; init; }
}