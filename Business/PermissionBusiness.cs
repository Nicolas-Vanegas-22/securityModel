using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<Permission> _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger<Permission> logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }

        // Método para obtener todos los permisos como DTOs
        public async Task<IEnumerable<PermissionDTO>> GetAllPermissionsAsync()
        {
            try
            {
                var permissions = await _permissionData.GetAllAsync();
                var permissionsDTO = new List<PermissionDTO>();

                foreach (var permission in permissions)
                {
                    permissionsDTO.Add(new PermissionDTO
                    {
                        PermissionId = permission.PermissionId,
                        PermissionName = permission.PermissionName,
                        Description = permission.Description
                    });
                }

                return permissionsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos", ex);
            }
        }

        // Método para obtener un permiso por ID como DTO
        public async Task<PermissionDTO> GetPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permiso con ID inválido: {PermissionId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("No se encontró ningún permiso con ID: {PermissionId}", id);
                    throw new EntityNotFoundException("Permiso", id);
                }

                return new PermissionDTO
                {
                    PermissionName = permission.PermissionName,
                    Description = permission.Description
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso con ID: {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }
        }

        // Método para crear un permiso desde un DTO
        public async Task<PermissionDTO> CreatePermissionAsync(PermissionDTO PermissionDto)
        {
            try
            {
                ValidatePermission(PermissionDto);

                var permission = new Permission
                {
                    PermissionName = PermissionDto.PermissionName,
                    Description = PermissionDto.Description
                };

                var permissionCreado = await _permissionData.CreateAsync(permission);

                return new PermissionDTO
                {
                    PermissionId = permissionCreado.PermissionId,
                    PermissionName = permissionCreado.PermissionName,
                    Description = permissionCreado.Description
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {Username}", PermissionDto?.PermissionName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidatePermission(PermissionDTO PermissionDto)
        {
            if (PermissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PermissionDto.PermissionName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del usuario es obligatorio");
            }
        }
    }
}