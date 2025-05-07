using System.Diagnostics;
using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<Rol> _logger;

        public RolBusiness(RolData rolData, ILogger<Rol> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<RolDTO>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                var rolesDTO = new List<RolDTO>();

                foreach (var rol in roles)
                {
                    rolesDTO.Add(new RolDTO
                    {
                        RolId = rol.RolId,
                        RolName = rol.RolName,
                        Description = rol.Description
                    });
                }

                return rolesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<RolDTO> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _rolData.GetByIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return new RolDTO
                {
                    RolId = rol.RolId,
                    RolName = rol.RolName,
                    Description = rol.Description
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<RolDTO> CreateRolAsync(RolDTO RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var rol = new Rol
                {
                    RolName = RolDto.RolName,
                    Description = RolDto.Description
                };

                rol.CreateAt = DateTime.Now;


                var rolCreado = await _rolData.CreateAsync(rol);

                return new RolDTO
                {
                    RolId = rolCreado.RolId,
                    RolName = rolCreado.RolName,
                    Description = rolCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", RolDto?.RolName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateRol(RolDTO RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(RolDto.RolName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }

        // Método para actualizar el usuario desde un DTO
        public async Task<RolDTO?> UpdateRolAsync(RolDTO RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var rolExistente = await _rolData.GetByIdAsync(RolDto.RolId);

                if (rolExistente == null)
                {
                    return null; // El controlador se encarga de devolver NotFound
                }

                rolExistente.RolName = RolDto.RolName;
                rolExistente.Description = RolDto.Description;

                var rolActualizado = await _rolData.UpdateRolAsync(rolExistente);

                return new RolDTO
                {
                    RolId = rolActualizado.RolId,
                    RolName = rolActualizado.RolName,
                    Description = rolActualizado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol: {RolId}", RolDto?.RolId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el rol", ex);
            }
        }

        // Método para eliminar un rol desde el DTO
        public async Task<bool> DeleteRolAsync(int id)
        {
            try
            {
                var rolExistente = await _rolData.GetByIdAsync(id);

                if (rolExistente == null)
                {
                    return false; // El rol no existe
                }

                var result = await _rolData.DeleteAsync(id);

                return result; // Devuelve true si se eliminó correctamente, false si no
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el rol con ID {id}", ex);
            }
        }

        // Elimina lógicamente un rol (soft delete).
        public async Task<bool> SoftDeleteRolAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var result = await _rolData.SoftDeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el eliminado lógico del rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar lógicamente el rol", ex);
            }
        }

        // Restaura un rol eliminado lógicamente.
        public async Task<bool> RestoreRolAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó restaurar un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var result = await _rolData.RestoreAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", "Error al restaurar el rol", ex);
            }
        }

        //Método para actualizar parcialmente los campos de un rol.

        public async Task<RolDTO?> PartialUpdateRolAsync(int id, RolDTO rolDto)
        {
            try
            {
                var existingRol = await _rolData.GetByIdAsync(id);

                if (existingRol == null)
                {
                    return null;
                }

                // Solo actualizamos los campos que vienen no nulos o no vacíos
                if (!string.IsNullOrWhiteSpace(rolDto.RolName))
                    existingRol.RolName = rolDto.RolName;

                if (!string.IsNullOrWhiteSpace(rolDto.Description))
                    existingRol.Description = rolDto.Description;

                var updatedRol = await _rolData.UpdateRolAsync(existingRol);

                return new RolDTO
                {
                    RolId = updatedRol.RolId,
                    RolName = updatedRol.RolName,
                    Description = updatedRol.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el rol con ID {id}", ex);
            }
        }

    }
}