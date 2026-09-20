namespace TP5Programacion.Compartidas.DTO.Paginado.Response;
// https://henriquesd.com/articles/pagination-in-a-net-web-api-with-ef-core
public record PaginadoResponseDto<T>(
    int NumeroPagina,
    int TamanoPagina,
    int TotalRegistros,
    int TotalPaginas,
    bool TienePaginaAnterior,
    bool TienePaginaPosterior,
    List<T> Datos);