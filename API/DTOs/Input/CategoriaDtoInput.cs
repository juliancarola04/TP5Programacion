namespace API.DTOs.Input
{
    public record CrearCategoriaDtoInput(
        string Nombre,
        string Descripcion);

    public record ActualizarCategoriaDtoInput(
        string Nombre,
        string Descripcion);
}
