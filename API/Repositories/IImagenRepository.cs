using API.Models;

namespace API.Repositories;

public interface IImagenRepository
{
    Task<Imagen?> ObtenerPorProductoId(int productoId);
    Task Crear(Imagen imagen);
    Task Eliminar(Imagen imagen);
}
