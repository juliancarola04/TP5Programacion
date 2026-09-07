using API.Data;
using API.Models;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion
{
    public class ProductoRepositoryPostgreSQL : IProductoRepository
    {
        private readonly DataContext _dataContext;
        public ProductoRepositoryPostgreSQL(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<List<Producto>> ObtenerTodos()
        {
            return await _dataContext.Productos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _dataContext.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> ExistePorNombre(string nombre)
        {
            return await _dataContext.Productos.AnyAsync(p => p.Nombre == nombre);
        }

        public async Task<bool> Existe(int id)
        {
            return await _dataContext.Productos.AnyAsync(p => p.Id == id);
        }

        public async Task Crear(Producto producto)
        {
            _dataContext.Productos.Add(producto);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Actualizar(Producto producto)
        {
            _dataContext.Productos.Update(producto);
            await _dataContext.SaveChangesAsync();
        }

        public async Task Eliminar(Producto producto)
        {
            _dataContext.Productos.Remove(producto);
            await _dataContext.SaveChangesAsync();
        }
    }
}
