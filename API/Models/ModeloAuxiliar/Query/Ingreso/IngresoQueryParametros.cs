using API.Models.ModeloAuxiliar.Query;

namespace API.Models.ModeloAuxiliar.Query.Ingreso;

public record IngresoQueryParametros : QueryParametros
{
    public int? ProveedorId { get; init; }
    public bool? Anulado { get; init; }
    public string? Buscar { get; init; } // Por razón social del proveedor
    public string? OrdenarPor { get; init; } = "fecha"; // "fecha", "total"
    public string? Direccion { get; init; } // "asc", "desc"
}
