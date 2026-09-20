namespace API.Models.ModeloAuxiliar.Query.Proveedor;

public record ProveedorQueryParametros : QueryParametros
{
    public bool? Eliminado { get; init; }
    public string? Buscar { get; init; } // Por razón social
    public string? OrdenarPor { get; init; } = "id"; // "Id", "razonsocial"
    public string? Direccion { get; init; } // "asc", "desc"=
}