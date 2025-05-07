using Business;
using Data;
using Entity.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de actividades en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ActivityController : ControllerBase
    {
        private readonly ActivityBusiness _ActivityBusiness;
        private readonly ILogger<ActivityController> _logger;

        /// <summary>
        /// Constructor del controlador de actividades
        /// </summary>
        public ActivityController(ActivityBusiness ActivityBusiness, ILogger<ActivityController> logger)
        {
            _ActivityBusiness = ActivityBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las actividades del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ActivityDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllActivity()
        {
            try
            {
                var activitys = await _ActivityBusiness.GetAllActivitysAsync();
                return Ok(activitys);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener actividades");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene una actividad específica por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ActivityDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetActivityById(int id)
        {
            try
            {
                var activity = await _ActivityBusiness.GetActivityByIdAsync(id);
                return Ok(activity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para actividad con ID: {ActivityId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Actividad no encontrado con ID: {ActivityId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener actividad con ID: {ActivityId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea una nueva actividad en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FormDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateDestination([FromBody] ActivityDTO ActivityDto)
        {
            try
            {
                var createdActivity = await _ActivityBusiness.CreateActivityAsync(ActivityDto);
                return CreatedAtAction(nameof(GetActivityById), new { id = createdActivity.ActivityId }, createdActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear actividad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear actividad");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        /// <summary>
        /// Actualiza los datos de una actividad existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ActivityDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateActivity(int id, [FromBody] ActivityDTO activityDto)
        {
            // Forzar que el ID en el DTO sea el mismo que el de la URL
            activityDto.ActivityId = id;

            try
            {
                var updatedActivity = await _ActivityBusiness.UpdateActivityAsync(activityDto);

                if (updatedActivity == null)
                {
                    return NotFound(new { message = $"No se encontró una actividad con ID {id}" });
                }

                return Ok(updatedActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar actividad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar adctividad");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina una actividad de la base de datos.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            try
            {
                var isDeleted = await _ActivityBusiness.DeleteActivityAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new { message = $"No se encontró una actividad con ID {id}" });
                }

                return Ok(new { message = "Actividad eliminada correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar actividad con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina lógicamente una actividad del sistema
        /// </summary>
        [HttpDelete("softdelete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteActivity(int id)
        {
            try
            {
                var result = await _ActivityBusiness.SoftDeleteActivityAsync(id);

                if (!result)
                {
                    return NotFound(new { message = $"No se encontró una actividad con ID {id}" });
                }

                return NoContent(); // Eliminado lógico exitoso
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar eliminar lógicamente actividad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente actividad");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Restaura una actividad eliminada lógicamente.
        /// </summary>
        [HttpPatch("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RestoreActivity(int id)
        {
            try
            {
                var result = await _ActivityBusiness.RestoreActivityAsync(id);

                if (!result)
                {
                    return NotFound(new { message = $"No se encontró una actividad con ID {id}" });
                }

                return Ok(new { message = $"Actividad restaurada correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar restaurar actividad");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al restaurar actividad");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente los datos de una actividad existente.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ActivityDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateRol(int id, [FromBody] ActivityDTO activityDto)
        {
            try
            {
                var updatedActivity = await _ActivityBusiness.PartialUpdateActivityAsync(id, activityDto);

                if (updatedActivity == null)
                {
                    return NotFound(new { message = $"No se encontró un usuario con ID {id}" });
                }

                return Ok(updatedActivity);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al modificar parte del usuario");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al modificar parte del usuario");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}