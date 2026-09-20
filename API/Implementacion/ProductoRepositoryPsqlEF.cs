using API.Data;
using API.Models;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Producto;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API.Implementacion
{
    public class ProductoRepositoryPsqlEF : IProductoRepository
    {
        private readonly DataContext _dataContext;
        public ProductoRepositoryPsqlEF(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<PaginadoResponse<Producto>> ObtenerTodos(ProductoQueryParametros parametros)
        {
            IQueryable<Producto> query = _dataContext.Productos
                .Include(p => p.Categoria)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(parametros.Buscar))
            {
                string buscar = parametros.Buscar.ToLower();
                query = query.Where(p => p.Nombre.ToLower().Contains(buscar));
            }

            if (parametros.CategoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == parametros.CategoriaId.Value);

            bool descendente = parametros.Direccion?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
            query = parametros.OrdenarPor?.ToLower() switch
            {
                "nombre" => descendente ? query.OrderByDescending(p => p.Nombre).ThenBy(p => p.Id) : query.OrderBy(p => p.Nombre).ThenBy(p => p.Id),
                "precio" => descendente ? query.OrderByDescending(p => p.PrecioVenta).ThenBy(p => p.Id) : query.OrderBy(p => p.PrecioVenta).ThenBy(p => p.Id),
                "stock" => descendente ? query.OrderByDescending(p => p.Stock).ThenBy(p => p.Id) : query.OrderBy(p => p.Stock).ThenBy(p => p.Id),
                _ => descendente ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id)
            };

            int totalRegistros = await query.CountAsync();
            List<Producto> productos = await query
                .Skip((parametros.NumeroPagina - 1) * parametros.TamanoPagina)
                .Take(parametros.TamanoPagina)
                .ToListAsync();

            return new PaginadoResponse<Producto>(productos, parametros.NumeroPagina, parametros.TamanoPagina, totalRegistros);
        }

        public async Task<Producto?> ObtenerPorId(int id)
        {
            return await _dataContext.Productos
                .Include(p => p.Categoria)
                .Include(p => p.Imagen)
                .FirstOrDefaultAsync(p => p.Id == id);
            //acá nunca agregar AsNoTracking porque va a romper el flujo de manejo del stock
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
