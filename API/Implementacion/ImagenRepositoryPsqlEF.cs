using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion
{
    public class ImagenRepositoryPsqlEF : IImagenRepository
    {
        private readonly DataContext _dataContext;

        public ImagenRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Imagen?> ObtenerPorProductoId(int productoId)
        {
            return await _dataContext.Imagenes
                .FirstOrDefaultAsync(i => i.ProductoId == productoId);
        }

        public async Task Crear(Imagen imagen)
        {
            _dataContext.Imagenes.Add(imagen);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Eliminar(Imagen imagen)
        {
            _dataContext.Imagenes.Remove(imagen);
            await _dataContext.SaveChangesAsync();
        }
    }
}
