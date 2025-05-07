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
    /// Controlador para la gestión de destinos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class DestinationController : ControllerBase
    {
        private readonly DestinationBusiness _DestinationBusiness;
        private readonly ILogger<DestinationController> _logger;

        /// <summary>
        /// Constructor del controlador de formularios
        /// </summary>
        public DestinationController(DestinationBusiness DestinationBusiness, ILogger<DestinationController> logger)
        {
            _DestinationBusiness = DestinationBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los destinos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllDestination()
        {
            try
            {
                var destinations = await _DestinationBusiness.GetAllDestinationsAsync();
                return Ok(destinations);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener destinos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un destino específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDestinationById(int id)
        {
            try
            {
                var form = await _DestinationBusiness.GetDestinationByIdAsync(id);
                return Ok(form);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para destino con ID: {DestinationId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Destino no encontrado con ID: {DestinationId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener destino con ID: {DestinationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo destino en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(FormDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateDestination([FromBody] DestinationDTO DestinationDto)
        {
            try
            {
                var createdDestination = await _DestinationBusiness.CreateDestinationAsync(DestinationDto);
                return CreatedAtAction(nameof(GetDestinationById), new { id = createdDestination.DestinationId }, createdDestination);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear destino");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear destino");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de un destino existente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DestinationDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateDestination(int id, [FromBody] DestinationDTO destinationDto)
        {
            // Forzar que el ID en el DTO sea el mismo que el de la URL
            destinationDto.DestinationId = id;

            try
            {
                var updatedDestination = await _DestinationBusiness.UpdateDestinationAsync(destinationDto);

                if (updatedDestination == null)
                {
                    return NotFound(new { message = $"No se encontró un destino con ID {id}" });
                }

                return Ok(updatedDestination);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar destino");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar destino");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un destino de la base de datos.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteDestination(int id)
        {
            try
            {
                var isDeleted = await _DestinationBusiness.DeleteDestinationAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new { message = $"No se encontró un destino con ID {id}" });
                }

                return Ok(new { message = "Destino eliminado correctamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar destino con ID: {DestinationId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimina lógicamente un destino del sistema
        /// </summary>
        [HttpDelete("softdelete/{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {
            try
            {
                var result = await _DestinationBusiness.SoftDeleteDestinationAsync(id);

                if (!result)
                {
                    return NotFound(new { message = $"No se encontró un destino con ID {id}" });
                }

                return NoContent(); // Eliminado lógico exitoso
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar eliminar lógicamente destino");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente destino");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Restaura un destino eliminado lógicamente.
        /// </summary>
        [HttpPatch("restore/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> RestoreDestination(int id)
        {
            try
            {
                var result = await _DestinationBusiness.RestoreDestinationAsync(id);

                if (!result)
                {
                    return NotFound(new { message = $"No se encontró un destino con ID {id}" });
                }

                return Ok(new { message = $"Destino restaurado correctamente" });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al intentar restaurar destino");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al restaurar destino");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza parcialmente los datos de un destino existente.
        /// </summary>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RolDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PartialUpdateDestination(int id, [FromBody] DestinationDTO destinationDto)
        {
            try
            {
                var updatedDestination = await _DestinationBusiness.PartialUpdateDestinationAsync(id, destinationDto);

                if (updatedDestination == null)
                {
                    return NotFound(new { message = $"No se encontró un destino con ID {id}" });
                }

                return Ok(updatedDestination);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al modificar parte del destino");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al modificar parte del destino");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}