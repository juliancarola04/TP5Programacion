using System.Data.Common;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;
using TP5Programacion.Compartidas.DTO.Proveedor.Request;
using TP5Programacion.Compartidas.DTO.Proveedor.Response;

namespace API.Services;

public class ProveedorService
{
    private readonly IProveedorRepository _repo;

    public ProveedorService(IProveedorRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<List<ObtenerProveedorResponse>> ObtenerTodos()
    {
        try
        {
            List<Proveedor> proveedores = await _repo.ObtenerTodos();

            List<ObtenerProveedorResponse> obtenerProveedoresResponse = proveedores.Select(
                p => new ObtenerProveedorResponse
                (
                    p.Id,
                    p.RazonSocial,
                    p.CUIT,
                    p.Direccion,
                    p.Email,
                    p.Telefono
                )).ToList();
                
            return obtenerProveedoresResponse;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<ObtenerProveedorResponse> ObtenerPorId(int id)
    {
        try
        {
            Proveedor? proveedor = await _repo.BuscarPorId(id);

            if (proveedor is null)
            {
                throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
            }

            ObtenerProveedorResponse obtenerProveedorResponse = new ObtenerProveedorResponse
            (
                proveedor.Id,
                proveedor.RazonSocial,
                proveedor.CUIT,
                proveedor.Direccion,
                proveedor.Email,
                proveedor.Telefono
                
            );
            
            return obtenerProveedorResponse;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<CrearProveedorResponse> Crear(CrearProveedorRequest dto)
    {
        if (!Validaciones.EstanDatosBien(dto.RazonSocial, dto.Cuit, dto.Direccion, dto.Email, dto.Telefono))
        {
            throw new DatosLlegaronErradosException("Todos los campos del proveedor son obligatorios.");
        }

        try
        {
            if (await _repo.ExistePorCuit(dto.Cuit))
            {
                throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
            }

            if (await _repo.ExistePorEmail(dto.Email))
            {
                throw new RecursoExistenteException("Ya existe un proveedor con ese email.");
            }

            Proveedor proveedor = new Proveedor
            {
                RazonSocial = dto.RazonSocial,
                CUIT = dto.Cuit,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion
            };

            await _repo.Crear(proveedor);

            CrearProveedorResponse proveedorDtoOutput = new CrearProveedorResponse
            (
                proveedor.Id,
                proveedor.RazonSocial,
                proveedor.CUIT,
                proveedor.Direccion,
                proveedor.Email,
                proveedor.Telefono
            );

            return proveedorDtoOutput;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task Actualizar(int id, ActualizarProveedorRequest actualizarProveedorRequest)
    {
        if (Validaciones.EstanDatosBien(id) == false)
        {
            throw new DatosLlegaronErradosException("El ID llegó errado.");
        }

        try
        {
            Proveedor? proveedor = await _repo.BuscarPorId(id);
            bool cambieAlgo = false;

            if (proveedor == null)
            {
                throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
            }

            if (actualizarProveedorRequest.RazonSocial != proveedor.RazonSocial && Validaciones.EstanDatosBien(proveedor.RazonSocial))
            {
                if (await _repo.ExistePorRazonSocial(actualizarProveedorRequest.RazonSocial!))
                {
                    throw new RecursoExistenteException("Ya existe alguien con esa razón social.");
                }
                else
                {
                    cambieAlgo = true;
                    proveedor.RazonSocial = actualizarProveedorRequest.RazonSocial!;
                }
            }

            if (actualizarProveedorRequest.Cuit != proveedor.CUIT && Validaciones.EstanDatosBien(proveedor.CUIT))
            {
                if (await _repo.ExistePorRazonSocial(actualizarProveedorRequest.Cuit!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
                }
                else
                {
                    cambieAlgo = true;
                    proveedor.CUIT = actualizarProveedorRequest.Cuit!;
                }
            }

            if (actualizarProveedorRequest.Direccion != proveedor.Direccion && Validaciones.EstanDatosBien(proveedor.Direccion))
            {
                cambieAlgo = true;
                proveedor.Direccion = actualizarProveedorRequest.Direccion!;
            }

            if (actualizarProveedorRequest.Email != proveedor.Email && Validaciones.EstanDatosBien(actualizarProveedorRequest.Email))
            {
                if (!Validaciones.EsUnEmailValido(actualizarProveedorRequest.Email!))
                {
                    throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
                }

                if (await _repo.ExistePorEmail(actualizarProveedorRequest.Email!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese E-Mail.");
                }

                cambieAlgo = true;
                proveedor.Email = actualizarProveedorRequest.Email!;
            }

            if (actualizarProveedorRequest.Telefono != proveedor.Telefono && Validaciones.EstanDatosBien(actualizarProveedorRequest.Telefono))
            {
                if (await _repo.ExistePorRazonSocial(actualizarProveedorRequest.Cuit!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
                }
                else
                {
                    cambieAlgo = true;
                    proveedor.Telefono = actualizarProveedorRequest.Telefono!;
                }

                cambieAlgo = true;
                proveedor.Email = actualizarProveedorRequest.Email!;
            }

            if (cambieAlgo == true)
            {
                await _repo.Actualizar(proveedor);
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
            Proveedor? proveedor = await _repo.BuscarPorId(id);

            if (proveedor == null)
            {
                throw new RecursoNoExisteException("No existe ningún usuario con ese ID.");
            }

            proveedor.Eliminado = true;

            await _repo.DarDeBaja(proveedor);
        }
        catch (DbException)
        {
            throw new BaseDeDatosException("Ocurrió un problema a la hora de contactar con la base de datos.");
        }
    }
}