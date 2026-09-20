namespace API.Models.ModeloAuxiliar.Query;

public record QueryParametros
{
    public int NumeroPagina { get; init; }
    public int TamanoPagina { get; init; }
}