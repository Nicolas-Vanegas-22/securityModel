using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class ActivityBusiness
    {
        private readonly ActivityData _activityData;
        private readonly ILogger<Activity> _logger;

        public ActivityBusiness(ActivityData activityData, ILogger<Activity> logger)
        {
            _activityData = activityData;
            _logger = logger;
        }

        // M�todo para obtener todas las actividades como DTOs
        public async Task<IEnumerable<ActivityDTO>> GetAllActivitysAsync()
        {
            try
            {
                var activitys  = await _activityData.GetAllAsync();
                var activitysDTO = new List<ActivityDTO>();

                foreach (var activity in activitys)
                {
                    activitysDTO.Add(new ActivityDTO
                    {
                        ActivityId = activity.ActivityId,
                        Name = activity.Name,
                        Description = activity.Description,
                        Category = activity.Category,
                        Price = activity.Price,
                        DurationHours = activity.DurationHours  
                    });
                }

                return activitysDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las actividades");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de actividades", ex);
            }
        }

        // M�todo para obtener una actividad por ID como DTO
        public async Task<ActivityDTO> GetActivityByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener una catividad con ID inv�lido: {ActivityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de actividad debe ser mayor que cero");
            }

            try
            {
                var activity = await _activityData.GetByIdAsync(id);
                if (activity == null)
                {
                    _logger.LogInformation("No se encontr� ning�na actividad con ID: {ActivityId}", id);
                    throw new EntityNotFoundException("Activity", id);
                }

                return new ActivityDTO
                {
                    ActivityId = activity.ActivityId,
                    Name = activity.Name,
                    Description = activity.Description,
                    Category = activity.Category,
                    Price = activity.Price
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la actividad con ID: {ActivityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la actividad con ID {id}", ex);
            }
        }

        // M�todo para crear la actividad desde un DTO
        public async Task<ActivityDTO> CreateActivityAsync(ActivityDTO ActivityDto)
        {
            try
            {
                ValidateActivity(ActivityDto);                
               

                var activity = new Activity
                {
                    Name = ActivityDto.Name,
                    Description = ActivityDto.Description,
                    Category = ActivityDto.Category,
                    Price = ActivityDto.Price,
                    DurationHours = ActivityDto.DurationHours

                };
                activity.CreateAt = DateTime.Now;


                var activityCreado = await _activityData.CreateAsync(activity);

                return new ActivityDTO
                {
                    ActivityId = activityCreado.ActivityId,
                    Name = activityCreado.Name,
                    Description = activityCreado.Description,
                    Category = activityCreado.Category,
                    Price = activityCreado.Price,
                    DurationHours = activityCreado.DurationHours
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva actividad: {Name}", ActivityDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la actividad", ex);
            }
        }

        // M�todo para validar el DTO
        private void ValidateActivity(ActivityDTO ActivityDto)
        {
            if (ActivityDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto actividad no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(ActivityDto.Name))
            {
                _logger.LogWarning("Se intent� crear/actualizar una actividad con Name vac�o");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la actividad es obligatorio");
            }
        }

        // M�todo para actualizar la actividad desde un DTO
        public async Task<ActivityDTO?> UpdateActivityAsync(ActivityDTO ActivityDto)
        {
            try
            {
                ValidateActivity(ActivityDto);

                var activityExistente = await _activityData.GetByIdAsync(ActivityDto.ActivityId);

                if (activityExistente == null)
                {
                    return null; // El controlador se encarga de devolver NotFound
                }

                activityExistente.Name = ActivityDto.Name;
                activityExistente.Description = ActivityDto.Description;
                activityExistente.Category = ActivityDto.Category;
                activityExistente.Price = ActivityDto.Price;
                activityExistente.DurationHours = ActivityDto.DurationHours;


                var activityActualizado = await _activityData.UpdateActivityAsync(activityExistente);

                return new ActivityDTO
                {
                    ActivityId = activityActualizado.ActivityId,
                    Name = activityActualizado.Name,
                    Description = activityActualizado.Description,
                    Category = activityActualizado.Category,
                    Price = activityActualizado.Price,
                    DurationHours = activityActualizado.DurationHours

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar actividad: {ActivityId}", ActivityDto?.ActivityId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al actualizar la actividad", ex);
            }
        }

        // M�todo para eliminar una actividad desde el DTO
        public async Task<bool> DeleteActivityAsync(int id)
        {
            try
            {
                var activityExistente = await _activityData.GetByIdAsync(id);

                if (activityExistente == null)
                {
                    return false; // El rol no existe
                }

                var result = await _activityData.DeleteAsync(id);

                return result; // Devuelve true si se elimin� correctamente, false si no
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar actividad con ID: {ActivityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar la actividad con ID {id}", ex);
            }
        }

        // Elimina l�gicamente una actividad (soft delete).
        public async Task<bool> SoftDeleteActivityAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� eliminar l�gicamente una actividad con ID inv�lido: {ActivityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la actividad debe ser mayor que cero");
            }

            try
            {
                var result = await _activityData.SoftDeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el eliminado l�gico de la actividad con ID: {ActivityId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar l�gicamente la actividad", ex);
            }
        }

        // Restaura una actividad eliminada l�gicamente.
        public async Task<bool> RestoreActivityAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� restaurar una actividad con ID inv�lido: {ActivityId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la actividad debe ser mayor que cero");
            }

            try
            {
                var result = await _activityData.RestoreAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar la actividad con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", "Error al restaurar la actividad", ex);
            }
        }
        //M�todo para actualizar parcialmente los campos de una actividad.

        public async Task<ActivityDTO?> PartialUpdateActivityAsync(int id, ActivityDTO activityDto)
        {
            try
            {
                var existingActivity = await _activityData.GetByIdAsync(id);

                if (existingActivity == null)
                {
                    return null;
                }

                // Solo actualizamos los campos que vienen no nulos o no vac�os
                if (!string.IsNullOrWhiteSpace(activityDto.Name))
                    existingActivity.Name = activityDto.Name;

                if (!string.IsNullOrWhiteSpace(activityDto.Description))
                    existingActivity.Description = activityDto.Description;

                if (!string.IsNullOrWhiteSpace(activityDto.Category))
                    existingActivity.Category = activityDto.Category;

                if (activityDto.Price > 0)
                    existingActivity.Price = activityDto.Price;

                if (activityDto.DurationHours != default)
                    existingActivity.DurationHours = activityDto.DurationHours;

                var updatedActivity = await _activityData.UpdateActivityAsync(existingActivity);

                return new ActivityDTO
                {
                    ActivityId = updatedActivity.ActivityId,
                    Name = updatedActivity.Name,
                    Description = updatedActivity.Description,
                    Category = updatedActivity.Category,
                    Price = updatedActivity.Price,
                    DurationHours = updatedActivity.DurationHours

                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente la actividad con ID: {ActivityId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente la actividad con ID {id}", ex);
            }
        }
    }
}