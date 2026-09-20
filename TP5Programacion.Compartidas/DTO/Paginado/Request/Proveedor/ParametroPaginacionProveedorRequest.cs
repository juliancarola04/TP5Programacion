namespace TP5Programacion.Compartidas.DTO.Paginado.Request.Usuario;

public record ParametroPaginacionProveedorRequest : ParametroPaginacionRequest
{
    public bool? Eliminado { get; set; }
    public string? Buscar { get; set; } // Por razón social
    public string? OrdenarPor { get; set; } = "id"; // "Id", "razonsocial"
    public string? Direccion { get; set; } // "asc", "desc"
}