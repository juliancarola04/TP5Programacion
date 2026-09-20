using API.Models.ModeloAuxiliar.Query;

namespace API.Models.ModeloAuxiliar.Query.Venta;

public record VentaQueryParametros : QueryParametros
{
    public int? ClienteId { get; init; }
    public bool? Anulada { get; init; }
    public string? Buscar { get; init; } // Por nombre del cliente
    public string? OrdenarPor { get; init; } = "fecha"; // "fecha", "total"
    public string? Direccion { get; init; } // "asc", "desc"
}
