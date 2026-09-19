using API.Excepciones;
using API.Repositories;
using API.Utilidades;
using API.Models;
using System.Data.Common;
using TP5Programacion.Compartidas.DTO.Usuario.Request;
using TP5Programacion.Compartidas.DTO.Usuario.Response;

namespace API.Services
{
    public class UsuarioService
    {
        public readonly IUsuarioRepository _repo;
        public readonly ITokenService _tokenService;

        public UsuarioService(IUsuarioRepository repo, ITokenService tokenService)
        {
            _repo = repo;
            _tokenService = tokenService;
        }

        public async Task ConvertirEnAdministrador(int id)
        {
            if (Validaciones.EstanDatosBien(id) == false)
            {
                throw new DatosLlegaronErradosException("El ID llegó errado.");
            }

            try
            {
                Usuario? usuario = await _repo.BuscarPorId(id);

                if (usuario == null)
                {
                    throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
                }

                usuario.EsAdministrador = true;

                await _repo.Actualizar(usuario);

            }
            catch (DbException)
            {
                throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
            }
        }

        public async Task QuitarAdministrador(int id)
        {
            if (Validaciones.EstanDatosBien(id) == false)
            {
                throw new DatosLlegaronErradosException("El ID llegó errado.");
            }

            try
            {
                Usuario? usuario = await _repo.BuscarPorId(id);

                if (usuario == null)
                {
                    throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
                }

                usuario.EsAdministrador = false;

                await _repo.Actualizar(usuario);

            }
            catch (DbException)
            {
                throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
            }
        }

        public async Task<ActualizarUsuarioResponse> Actualizar(int id, ActualizarUsuarioRequest actualizarUsuarioRequest)
        {
            if (Validaciones.EstanDatosBien(id) == false)
            {
                throw new DatosLlegaronErradosException("El ID llegó errado.");
            }

            try
            {
                Usuario? usuario = await _repo.BuscarPorId(id);
                bool cambieAlgo = false;

                if (usuario == null)
                {
                    throw new RecursoNoExisteException("No existe ningún proveedor con ese ID.");
                }

                if (actualizarUsuarioRequest.Username != usuario.Username && Validaciones.EstanDatosBien(actualizarUsuarioRequest.Username))
                {
                    if (await _repo.ExistePorUsername(actualizarUsuarioRequest.Username!))
                    {
                        throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
                    }
                    else
                    {
                        cambieAlgo = true;
                        usuario.Username = actualizarUsuarioRequest.Username!;
                    }

                }

                if (actualizarUsuarioRequest.Email != usuario.Email && Validaciones.EstanDatosBien(actualizarUsuarioRequest.Email))
                {
                    if (!Validaciones.EsUnEmailValido(actualizarUsuarioRequest.Email!))
                    {
                        throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
                    }

                    if (await _repo.ExistePorEmail(actualizarUsuarioRequest.Email!))
                    {
                        throw new RecursoExistenteException("Ya existe un usuario con ese E-Mail.");
                    }

                    cambieAlgo = true;
                    usuario.Email = actualizarUsuarioRequest.Email!;
                }

                if (Validaciones.EstanDatosBien(actualizarUsuarioRequest.Password) && !BCrypt.Net.BCrypt.EnhancedVerify(actualizarUsuarioRequest.Password, usuario.Password))
                {
                    cambieAlgo = true;
                    usuario.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(actualizarUsuarioRequest.Password);
                }

                if (cambieAlgo == true)
                {
                    await _repo.Actualizar(usuario);
                    
                    (string token, DateTime expiracion) = _tokenService.CrearToken(usuario);

                    ActualizarUsuarioResponse actualizarUsuarioResponse = new ActualizarUsuarioResponse(token);

                    return actualizarUsuarioResponse;
                }
                else
                {
                    throw new DatosLlegaronErradosException("Los datos que mandó fueron inválidos");
                }
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Pasó un problema y no se pudo actualizar el usuario: {e.Message}");
            }
        }

        public async Task DarDeBaja(int id)
        {
            if (Validaciones.EstanDatosBien(id) == false)
            {
                throw new DatosLlegaronErradosException("El ID llegó errado.");
            }

            try
            {
                Usuario? usuario = await _repo.BuscarPorId(id);

                if (usuario == null)
                {
                    throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
                }

                usuario.Eliminado = true;

                await _repo.DarDeBaja(usuario);
            }
            catch (DbException)
            {
                throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
            }
        }

        public async Task<List<ObtenerUsuarioResponse>> ObtenerlosATodos()
        {
            try
            {
                List<Usuario> usuarios = await _repo.ObtenerTodos();
                
                List<ObtenerUsuarioResponse> usuariosDtoOutputs = usuarios.Select(
                    u => new ObtenerUsuarioResponse
                    (
                        u.Id,
                        u.Username,
                        u.Email,
                        u.EsAdministrador
                    )).ToList();
                
                return usuariosDtoOutputs;
            }
            catch (DbException)
            {
                throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
            }
        }
    }
}
