namespace API.Models.ModeloAuxiliar;
// https://github.com/henriquesd/PaginationDemo/blob/master/src/PaginationDemo.Domain/Models/PagedResponseOffset.cs
// y también sacao de las diapos de Fede.

public record PaginadoResponse<T>
{
    public int NumeroPagina { get; init; }
    public int TamanoPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
    public bool TienePaginaAnterior => NumeroPagina > 1;
    public bool TienePaginaPosterior => NumeroPagina < TotalPaginas;
    public List<T> Datos { get; init; }

    public PaginadoResponse(List<T> datos, int numeroPagina, int tamanoPagina, int totalRegistros)
    {
        Datos = datos;
        NumeroPagina = numeroPagina;
        TamanoPagina = tamanoPagina;
        TotalRegistros = totalRegistros;
        TotalPaginas = (int)Math.Ceiling((decimal)totalRegistros / (decimal)tamanoPagina);

    }
}