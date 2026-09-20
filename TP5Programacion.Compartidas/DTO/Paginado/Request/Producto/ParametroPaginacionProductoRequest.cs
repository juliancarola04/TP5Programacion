using TP5Programacion.Compartidas.DTO.Paginado.Request;

namespace TP5Programacion.Compartidas.DTO.Paginado.Request.Producto;

public record ParametroPaginacionProductoRequest : ParametroPaginacionRequest
{
    public string? Buscar { get; init; } // Por nombre
    public int? CategoriaId { get; init; }
    public string? OrdenarPor { get; init; } // "id", "nombre", "precio", "stock"
    public string? Direccion { get; init; } // "asc", "desc"
}
