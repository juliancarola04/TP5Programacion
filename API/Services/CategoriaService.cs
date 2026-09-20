using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using TP5Programacion.Compartidas.DTO.Categoria.Request;
using TP5Programacion.Compartidas.DTO.Categoria.Response;
using API.Models.ModeloAuxiliar;
using API.Models.ModeloAuxiliar.Query.Categoria;
using TP5Programacion.Compartidas.DTO.Paginado.Request.Categoria;


namespace API.Services
{
    public class CategoriaService
    {
        private readonly ICategoriaRepository _repo;
        public CategoriaService(ICategoriaRepository repo)
        {
            _repo = repo;
        }
        public async Task<PaginadoResponse<CategoriaResponse>> ObtenerTodas(ParametroPaginacionCategoriaRequest parametros)
        {
            try
            {
                int numeroPagina = parametros.NumeroPagina is null || parametros.NumeroPagina < 1
                    ? 1
                    : parametros.NumeroPagina.Value;

                int tamanoPagina = parametros.TamanoPagina is null || parametros.TamanoPagina < 1
                    ? 20
                    : parametros.TamanoPagina > 50 ? 50 : parametros.TamanoPagina.Value;

                string? direccion =
                    string.IsNullOrWhiteSpace(parametros.Direccion) &&
                    parametros.Direccion?.ToLower() is not ("asc" or "desc")
                        ? "desc"
                        : parametros.Direccion;

                string? buscar = parametros.Buscar?.Trim().ToLower();

                string? ordenarPor = string.IsNullOrWhiteSpace(parametros.OrdenarPor) ? null : parametros.OrdenarPor;

                CategoriaQueryParametros categoriaQueryParametros = new CategoriaQueryParametros
                {
                    NumeroPagina = numeroPagina,
                    TamanoPagina = tamanoPagina,
                    Buscar = buscar,
                    OrdenarPor = ordenarPor,
                    Direccion = direccion
                };

                PaginadoResponse<Categoria> resultado = await _repo.ObtenerTodas(categoriaQueryParametros);

                List<CategoriaResponse> categorias = resultado.Datos.Select(c => new CategoriaResponse(
                    c.Id, c.Nombre, c.Descripcion
                )).ToList();

                return new PaginadoResponse<CategoriaResponse>(
                    categorias,
                    resultado.NumeroPagina,
                    resultado.TamanoPagina,
                    resultado.TotalRegistros
                );
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task<CategoriaResponse> ObtenerPorId(int id)
        {
            try
            {
                Categoria? categoria = await _repo.ObtenerPorId(id);

                if (categoria is null)
                {
                    throw new RecursoNoExisteException("No existe ninguna categoría con ese id.");
                }

                return new CategoriaResponse(categoria.Id, categoria.Nombre, categoria.Descripcion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task<CategoriaResponse> Crear(CrearCategoriaRequest dto)
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

                return new CategoriaResponse(categoria.Id, categoria.Nombre, categoria.Descripcion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
        public async Task Actualizar(int id, ActualizarCategoriaRequest dto)
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

