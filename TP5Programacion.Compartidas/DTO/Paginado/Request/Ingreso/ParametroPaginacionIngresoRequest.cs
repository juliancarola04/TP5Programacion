using TP5Programacion.Compartidas.DTO.Paginado.Request;

namespace TP5Programacion.Compartidas.DTO.Paginado.Request.Ingreso;

public record ParametroPaginacionIngresoRequest : ParametroPaginacionRequest
{
    public int? ProveedorId { get; init; }
    public bool? Anulado { get; init; }
    public string? Buscar { get; init; } // Por razón social del proveedor
    public string? OrdenarPor { get; init; } // "fecha", "total"
    public string? Direccion { get; init; } // "asc", "desc"
}
