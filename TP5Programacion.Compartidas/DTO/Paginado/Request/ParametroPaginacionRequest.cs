namespace TP5Programacion.Compartidas.DTO.Paginado.Request;

public record ParametroPaginacionRequest
{
    public int? NumeroPagina { get; init; }
    public int? TamanoPagina { get; init; }
}
    