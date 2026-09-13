using API.Excepciones;
using API.Repositories;
using API.Utilidades;
using API.Models;
using API.DTOs.Input;
using System.Data.Common;
using API.DTOs.Output;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        public async Task<UsuarioAuthDtoOutput> Actualizar(int id, UsuarioDtoInput usuarioDtoInput)
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

                if (usuarioDtoInput.Username != usuario.Username && Validaciones.EstanDatosBien(usuarioDtoInput.Username))
                {
                    if (await _repo.ExistePorUsername(usuarioDtoInput.Username!))
                    {
                        throw new RecursoExistenteException("Ya existe alguien con ese usuario.");
                    }
                    else
                    {
                        cambieAlgo = true;
                        usuario.Username = usuarioDtoInput.Username!;
                    }

                }

                if (usuarioDtoInput.Email != usuario.Email && Validaciones.EstanDatosBien(usuarioDtoInput.Email))
                {
                    if (!Validaciones.EsUnEmailValido(usuarioDtoInput.Email!))
                    {
                        throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
                    }

                    if (await _repo.ExistePorEmail(usuarioDtoInput.Email!))
                    {
                        throw new RecursoExistenteException("Ya existe un usuario con ese E-Mail.");
                    }

                    cambieAlgo = true;
                    usuario.Email = usuarioDtoInput.Email!;
                }

                if (Validaciones.EstanDatosBien(usuarioDtoInput.Password) && !BCrypt.Net.BCrypt.EnhancedVerify(usuarioDtoInput.Password, usuario.Password))
                {
                    cambieAlgo = true;
                    usuario.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(usuarioDtoInput.Password);
                }

                if (cambieAlgo == true)
                {
                    await _repo.Actualizar(usuario);
                    
                    (string token, DateTime expiracion) = _tokenService.CrearToken(usuario);

                    UsuarioAuthDtoOutput usuarioDtoOutput = new UsuarioAuthDtoOutput()
                    {
                        Token = token,
                        Expiracion = expiracion
                    };

                    return usuarioDtoOutput;
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

        public async Task<List<UsuarioDtoOutput>> ObtenerlosATodos()
        {
            try
            {
                List<Usuario> usuarios = await _repo.ObtenerTodos();
                
                List<UsuarioDtoOutput> usuariosDtoOutputs = usuarios.Select(
                    u => new UsuarioDtoOutput
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Email = u.Email,
                        EsAdministrador = u.EsAdministrador
                    }).ToList();
                
                return usuariosDtoOutputs;
            }
            catch (DbException)
            {
                throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
            }
        }
    }
}
