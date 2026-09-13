using System.Data.Common;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;

namespace API.Services
{
    public class ClienteService
    {
        private readonly IClienteRepository _repo;

        public ClienteService(IClienteRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<ClienteDtoOutput>> ObtenerTodos()
        {
            try
            {
                List<Cliente> clientes = await _repo.ObtenerTodos();

                return clientes.Select(c => new ClienteDtoOutput(
                    c.Id, c.Nombre, c.Dni, c.Telefono, c.Email, c.Direccion
                )).ToList();
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task<ClienteDtoOutput> ObtenerPorId(int id)
        {
            try
            {
                Cliente? cliente = await _repo.ObtenerPorId(id);

                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                return new ClienteDtoOutput(cliente.Id, cliente.Nombre, cliente.Dni, cliente.Telefono, cliente.Email, cliente.Direccion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task<ClienteDtoOutput> Crear(CrearClienteDtoInput dto)
        {
            if (!Validaciones.EstanDatosBien(dto.Nombre, dto.Dni, dto.Telefono, dto.Email, dto.Direccion))
            {
                throw new DatosLlegaronErradosException("Todos los campos del cliente son obligatorios.");
            }

            if (dto.Nombre.Length > 30 || dto.Dni.Length > 8 || dto.Telefono.Length > 13
                || dto.Email.Length > 320 || dto.Direccion.Length > 40)
            {
                throw new DatosLlegaronErradosException("Alguno de los campos excede la longitud máxima permitida.");
            }

            try
            {
                if (await _repo.ExistePorDni(dto.Dni))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese DNI.");
                }

                if (await _repo.ExistePorEmail(dto.Email))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese email.");
                }

                Cliente cliente = new Cliente
                {
                    Nombre = dto.Nombre,
                    Dni = dto.Dni,
                    Telefono = dto.Telefono,
                    Email = dto.Email,
                    Direccion = dto.Direccion
                };

                await _repo.Crear(cliente);

                return new ClienteDtoOutput(cliente.Id, cliente.Nombre, cliente.Dni, cliente.Telefono, cliente.Email, cliente.Direccion);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarClienteDtoInput dto)
        {
            if (!Validaciones.EstanDatosBien(dto.Nombre, dto.Dni, dto.Telefono, dto.Email, dto.Direccion))
            {
                throw new DatosLlegaronErradosException("Todos los campos del cliente son obligatorios.");
            }

            try
            {
                Cliente? cliente = await _repo.ObtenerPorId(id);

                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                // Si cambió el DNI o el Email, hay que re-chequear unicidad (evitar chocar con otro cliente existente)
                if (cliente.Dni != dto.Dni && await _repo.ExistePorDni(dto.Dni))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese DNI.");
                }

                if (cliente.Email != dto.Email && await _repo.ExistePorEmail(dto.Email))
                {
                    throw new RecursoExistenteException("Ya existe un cliente con ese email.");
                }

                cliente.Nombre = dto.Nombre;
                cliente.Dni = dto.Dni;
                cliente.Telefono = dto.Telefono;
                cliente.Email = dto.Email;
                cliente.Direccion = dto.Direccion;

                await _repo.Actualizar(cliente);
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
                Cliente? cliente = await _repo.ObtenerPorId(id);

                if (cliente is null)
                {
                    throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
                }

                if (await _repo.TieneVentasAsociadas(id))
                {
                    throw new RecursoExistenteException("No se puede eliminar el cliente porque tiene ventas asociadas.");
                }

                await _repo.Eliminar(cliente);
            }
            catch (DbException e)
            {
                throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
            }
        }
    }
}
