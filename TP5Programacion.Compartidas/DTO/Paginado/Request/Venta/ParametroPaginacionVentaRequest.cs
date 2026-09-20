using TP5Programacion.Compartidas.DTO.Paginado.Request;

namespace TP5Programacion.Compartidas.DTO.Paginado.Request.Venta;

public record ParametroPaginacionVentaRequest : ParametroPaginacionRequest
{
    public int? ClienteId { get; init; }
    public bool? Anulada { get; init; }
    public string? Buscar { get; init; } // Por nombre del cliente
    public string? OrdenarPor { get; init; } // "fecha", "total"
    public string? Direccion { get; init; } // "asc", "desc"
}
