using API.Repositories;
using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
namespace API.Implementacion
{
    public class CategoriaRepositoryPostgreSQL : ICategoriaRepository
    {
        private readonly DataContext _dataContext;
        public CategoriaRepositoryPostgreSQL (DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<List<Categoria>> ObtenerTodas()
        {
            return await _dataContext.Categorias
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<Categoria?> ObtenerPorId(int id)
        {
            return await _dataContext.Categorias
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> ExistePorNombre(string nombre)
        {
            return await _dataContext.Categorias.AnyAsync(c => c.Nombre == nombre);
        }
        public async Task<bool> TieneProductosAsociados(int categoriaId)
        {
            return await _dataContext.Productos.AnyAsync(p => p.CategoriaId == categoriaId);
        }
        public async Task Crear(Categoria categoria)
        {
            _dataContext.Categorias.Add(categoria);
            await _dataContext.SaveChangesAsync();
        }
        public async Task Actualizar(Categoria categoria)
        {
            _dataContext.Categorias.Update(categoria);
            await _dataContext.SaveChangesAsync();
        }
        public async Task Eliminar(Categoria categoria)
        {
            _dataContext.Categorias.Remove(categoria);
            await _dataContext.SaveChangesAsync();
        }

    }
}
