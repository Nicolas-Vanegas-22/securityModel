using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class ModuleBusiness
    {
        private readonly ModuleData _moduleData;
        private readonly ILogger<Module> _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger<Module> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        // M�todo para obtener todos los modulos como DTOs
        public async Task<IEnumerable<ModuleDTO>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                var modulesDTO = new List<ModuleDTO>();
                
                foreach (var module in modules)
                {
                    modulesDTO.Add(new ModuleDTO
                    {
                        ModuleId = module.ModuleId,
                        Name = module.Name
                    });
                }

                return modulesDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los modulos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modulos", ex);
            }
        }

        // M�todo para obtener un modulo por ID como DTO
        public async Task<ModuleDTO> GetModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un modulo con ID inv�lido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del modulo debe ser mayor que cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogInformation("No se encontr� ning�n modulo con ID: {ModuleId}", id);
                    throw new EntityNotFoundException("Modulo", id);
                }

                return new ModuleDTO
                {
                    ModuleId = module.ModuleId,
                    Name = module.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el modulo con ID: {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el modulo con ID {id}", ex);
            }
        }

        // M�todo para crear un modulo desde un DTO
        public async Task<ModuleDTO> CreateModuleAsync(ModuleDTO ModuleDto)
        {
            try
            {
                ValidateModule(ModuleDto);

                var module = new Module
                {
                    Name = ModuleDto.Name,
                };

                var moduleCreado = await _moduleData.CreateAsync(module);

                return new ModuleDTO
                {
                    ModuleId = moduleCreado.ModuleId,
                    Name = moduleCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo modulo: {Name}", ModuleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el modulo", ex);
            }
        }

        // M�todo para validar el DTO
        private void ValidateModule(ModuleDTO ModuleDto)
        {
            if (ModuleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto modulo no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ModuleDto.Name))
            {
                _logger.LogWarning("Se intent� crear/actualizar un modulo con Name vac�o");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del modulo es obligatorio");
            }
        }
    }
}