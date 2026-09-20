using TP5Programacion.Compartidas.DTO.Paginado.Request;

namespace TP5Programacion.Compartidas.DTO.Paginado.Request.Cliente;

public record ParametroPaginacionClienteRequest : ParametroPaginacionRequest
{
    public string? Buscar { get; init; } // Por nombre, DNI o E-Mail
    public string? OrdenarPor { get; init; } // "id", "nombre", "email"
    public string? Direccion { get; init; } // "asc", "desc"
}
