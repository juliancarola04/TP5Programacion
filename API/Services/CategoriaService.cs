using System.Data.Common;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;

namespace API.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _repo;
        public CategoriaService(ICategoriaRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<CategoriaDtoOutput>> ObtenerTodas()
        {
            try
            {
                List<Categoria> categorias = await _repo.ObtenerTodas();

                return categorias.Select(c => new CategoriaDtoOutput(
                    c.Id, c.Nombre, c.Descripcion
                )).ToList();
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task<CategoriaDtoOutput> ObtenerPorId(int id)
        {
            try
            {
                Categoria? categoria = await _repo.ObtenerPorId(id);

                if (categoria is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna categoría con ese id.");
                }

                return new CategoriaDtoOutput(categoria.Id, categoria.Nombre, categoria.Descripcion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task<CategoriaDtoOutput> Crear(CrearCategoriaDtoInput dto)
        {
            if (!Validaciones.EstanDatosBien(dto.Nombre, dto.Descripcion))
            {
                throw new DatosLlegaronErradosException("El nombre y la descripción de la categoría son obligatorios.");
            }

            try
            {
                if (await _repo.ExistePorNombre(dto.Nombre))
                {
                    throw new RecursoExistenteException("Ya existe una categoría con ese nombre.");
                }

                Categoria categoria = new Categoria
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion
                };

                await _repo.Crear(categoria);

                return new CategoriaDtoOutput(categoria.Id, categoria.Nombre, categoria.Descripcion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task Actualizar(int id, ActualizarCategoriaDtoInput dto)
        {
            if (!Validaciones.EstanDatosBien(dto.Nombre, dto.Descripcion))
            {
                throw new DatosLlegaronErradosException("El nombre y la descripción de la categoría son obligatorios.");
            }

            try
            {
                Categoria? categoria = await _repo.ObtenerPorId(id);

                if (categoria is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna categoría con ese id.");
                }

                categoria.Nombre = dto.Nombre;
                categoria.Descripcion = dto.Descripcion;

                await _repo.Actualizar(categoria);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task Eliminar(int id)
        {
            try
            {
                Categoria? categoria = await _repo.ObtenerPorId(id);

                if (categoria is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna categoría con ese id.");
                }

                if (await _repo.TieneProductosAsociados(id))
                {
                    throw new RecursoExistenteException("No se puede eliminar la categoría porque tiene productos asociados.");
                }

                await _repo.Eliminar(categoria);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
    }
}

