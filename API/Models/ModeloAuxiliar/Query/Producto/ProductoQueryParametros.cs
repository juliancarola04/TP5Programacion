using API.Models.ModeloAuxiliar.Query;

namespace API.Models.ModeloAuxiliar.Query.Producto;

public record ProductoQueryParametros : QueryParametros
{
    public string? Buscar { get; init; } // Por nombre
    public int? CategoriaId { get; init; }
    public string? OrdenarPor { get; init; } = "id"; // "id", "nombre", "precio", "stock"
    public string? Direccion { get; init; } // "asc", "desc"
}
