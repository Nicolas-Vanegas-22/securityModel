using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class ChangeLogBusiness
    {
        private readonly ChangeLogData _changeLogData;
        private readonly ILogger<ChangeLog> _logger;

        public ChangeLogBusiness(ChangeLogData changeLogData, ILogger<ChangeLog> logger)
        {
            _changeLogData = changeLogData;
            _logger = logger;
        }

        // Método para obtener todos los cambios como DTOs
        public async Task<IEnumerable<ChangeLogDTO>> GetAllChangeLogsAsync()
        {
            try
            {
                var changesLogs = await _changeLogData.GetAllAsync();
                var changesLogsDTO = new List<ChangeLogDTO>();

                foreach (var changeLog in changesLogs)
                {
                    changesLogsDTO.Add(new ChangeLogDTO
                    {
                        ChangeLogId = changeLog.ChangeLogId,
                        Description = changeLog.Description,
                        ChangeDate = changeLog.ChangeDate
                    });
                }

                return changesLogsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los cambios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de cambios", ex);
            }
        }

        // Método para obtener un cambio por ID como DTO
        public async Task<ChangeLogDTO> GetChangeLogByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un cambio con ID inválido: {ChangeLogId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del cambio debe ser mayor que cero");
            }

            try
            {
                var changeLog = await _changeLogData.GetByIdAsync(id);
                if (changeLog == null)
                {
                    _logger.LogInformation("No se encontró ningún cambio con ID: {ChangeLogId}", id);
                    throw new EntityNotFoundException("Usuario", id);
                }

                return new ChangeLogDTO
                {
                    ChangeLogId = changeLog.ChangeLogId,
                    Description = changeLog.Description,
                    ChangeDate = changeLog.ChangeDate
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el cambio con ID: {ChangeLogId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el cambio con ID {id}", ex);
            }
        }

        // Método para crear un cambio desde un DTO
        public async Task<ChangeLogDTO> CreateChangeLogAsync(ChangeLogDTO ChangeLogDto)
        {
            try
            {
                ValidateChangeLog(ChangeLogDto);

                var changeLog = new ChangeLog
                {
                    Description = ChangeLogDto.Description,
                    ChangeDate = ChangeLogDto.ChangeDate
                };

                var changeLogCreado = await _changeLogData.CreateAsync(changeLog);

                return new ChangeLogDTO
                {
                    ChangeLogId = changeLogCreado.ChangeLogId,
                    Description = changeLogCreado.Description,
                    ChangeDate = changeLogCreado.ChangeDate
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo cambio: {Description}", ChangeLogDto?.Description ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el cambio", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateChangeLog(ChangeLogDTO ChangeLogDto)
        {
            if (ChangeLogDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto cambio no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ChangeLogDto.Description))
            {
                _logger.LogWarning("Se intentó crear/actualizar un cambio con Description vacío");
                throw new Utilities.Exceptions.ValidationException("Description", "El Description del cambio es obligatorio");
            }
        }
    }
}