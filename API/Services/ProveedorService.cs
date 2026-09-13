using System.Data.Common;
using API.DTOs.Input;
using API.DTOs.Output;
using API.Excepciones;
using API.Models;
using API.Repositories;
using API.Utilidades;

namespace API.Services;

public class ProveedorService
{
    private readonly IProveedorRepository _repo;

    public ProveedorService(IProveedorRepository repo)
    {
        _repo = repo;
    }
    
    public async Task<List<ProveedorDtoOutput>> ObtenerTodos()
    {
        try
        {
            List<Proveedor> proveedores = await _repo.ObtenerTodos();

            List<ProveedorDtoOutput> proveedorDtoOutputs = proveedores.Select(
                p => new ProveedorDtoOutput
                {
                    Id = p.Id,
                    RazonSocial = p.RazonSocial,
                    Cuit = p.CUIT,
                    Direccion = p.Direccion,
                    Email = p.Email,
                    Telefono = p.Telefono
                }).ToList();
                
            return proveedorDtoOutputs;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<ProveedorDtoOutput> ObtenerPorId(int id)
    {
        try
        {
            Proveedor? proveedor = await _repo.BuscarPorId(id);

            if (proveedor is null)
            {
                throw new RecursoNoExisteException("No existe ningún cliente con ese id.");
            }

            ProveedorDtoOutput proveedorDtoOutput = new ProveedorDtoOutput
            {
                Id = proveedor.Id,
                RazonSocial = proveedor.RazonSocial,
                Cuit = proveedor.CUIT,
                Direccion = proveedor.Direccion,
                Email = proveedor.Email,
                Telefono = proveedor.Telefono
                
            };
            
            return proveedorDtoOutput;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task<ProveedorDtoOutput> Crear(ProveedorDtoInput dto)
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

            ProveedorDtoOutput proveedorDtoOutput = new ProveedorDtoOutput
            {
                Id = proveedor.Id,
                RazonSocial = proveedor.RazonSocial,
                Cuit = proveedor.CUIT,
                Telefono = proveedor.Telefono,
                Email = proveedor.Email,
                Direccion = proveedor.Direccion
            };

            return proveedorDtoOutput;
        }
        catch (DbException e)
        {
            throw new BaseDeDatosException($"Ocurrió un problema: {e.Message}");
        }
    }

    public async Task Actualizar(int id, ProveedorDtoInput proveedorDtoInput)
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

            if (proveedorDtoInput.RazonSocial != proveedor.RazonSocial && Validaciones.EstanDatosBien(proveedor.RazonSocial))
            {
                if (await _repo.ExistePorRazonSocial(proveedorDtoInput.RazonSocial!))
                {
                    throw new RecursoExistenteException("Ya existe alguien con esa razón social.");
                }
                else
                {
                    cambieAlgo = true;
                    proveedor.RazonSocial = proveedorDtoInput.RazonSocial!;
                }
            }

            if (proveedorDtoInput.Cuit != proveedor.CUIT && Validaciones.EstanDatosBien(proveedor.CUIT))
            {
                if (await _repo.ExistePorRazonSocial(proveedorDtoInput.Cuit!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
                }
                else
                {
                    cambieAlgo = true;
                    proveedor.CUIT = proveedorDtoInput.Cuit!;
                }
            }

            if (proveedorDtoInput.Direccion != proveedor.Direccion && Validaciones.EstanDatosBien(proveedor.Direccion))
            {
                cambieAlgo = true;
                proveedor.Direccion = proveedorDtoInput.Direccion!;
            }

            if (proveedorDtoInput.Email != proveedor.Email && Validaciones.EstanDatosBien(proveedorDtoInput.Email))
            {
                if (!Validaciones.EsUnEmailValido(proveedorDtoInput.Email!))
                {
                    throw new DatosLlegaronErradosException("El formato del E-Mail es inválido.");
                }

                if (await _repo.ExistePorEmail(proveedorDtoInput.Email!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese E-Mail.");
                }

                cambieAlgo = true;
                proveedor.Email = proveedorDtoInput.Email!;
            }

            if (proveedorDtoInput.Telefono != proveedor.Telefono && Validaciones.EstanDatosBien(proveedorDtoInput.Telefono))
            {
                if (await _repo.ExistePorRazonSocial(proveedorDtoInput.Cuit!))
                {
                    throw new RecursoExistenteException("Ya existe un proveedor con ese CUIT.");
                }
                else
                {
                    cambieAlgo = true;
                    proveedor.Telefono = proveedorDtoInput.Telefono!;
                }

                cambieAlgo = true;
                proveedor.Email = proveedorDtoInput.Email!;
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