using API.Models.ModeloAuxiliar.Query;

namespace API.Models.ModeloAuxiliar.Query.Categoria;

public record CategoriaQueryParametros : QueryParametros
{
    public string? Buscar { get; init; } // Por nombre
    public string? OrdenarPor { get; init; } = "id"; // "id", "nombre"
    public string? Direccion { get; init; } // "asc", "desc"
}
