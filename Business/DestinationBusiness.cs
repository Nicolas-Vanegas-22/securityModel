using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class DestinationBusiness
    {
        private readonly DestinationData _destinationData;
        private readonly ILogger<Destination> _logger;

        public DestinationBusiness(DestinationData destinationData, ILogger<Destination> logger)
        {
            _destinationData = destinationData;
            _logger = logger;
        }

        // Método para obtener todos los destinos como DTOs
        public async Task<IEnumerable<DestinationDTO>> GetAllDestinationsAsync()
        {
            try
            {
                var destinations = await _destinationData.GetAllAsync();
                var destinationsDTO = new List<DestinationDTO>();

                foreach (var destination in destinations)
                {
                    destinationsDTO.Add(new DestinationDTO
                    {
                        DestinationId = destination.DestinationId,
                        Name = destination.Name,
                        Description = destination.Description,
                        Country = destination.Country,
                        Region = destination.Region,
                        Latitude = destination.Latitude,
                        Longitude = destination.Longitude
                    });
                }

                return destinationsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los destinos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de destinos", ex);
            }
        }

        // Método para obtener un destino por ID como DTO
        public async Task<DestinationDTO> GetDestinationByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un destino con ID inválido: {DestinationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del destino debe ser mayor que cero");
            }

            try
            {
                var destination = await _destinationData.GetByIdAsync(id);
                if (destination == null)
                {
                    _logger.LogInformation("No se encontró ningún destino con ID: {DestinationId}", id);
                    throw new EntityNotFoundException("Destination", id);
                }

                return new DestinationDTO
                {
                    DestinationId = destination.DestinationId,
                    Name = destination.Name,
                    Description = destination.Description,
                    Country = destination.Country,
                    Region = destination.Region,
                    Latitude = destination.Latitude,
                    Longitude = destination.Longitude
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el destino con ID: {DestinationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el destino con ID {id}", ex);
            }
        }

        // Método para crear un destino desde un DTO
        public async Task<DestinationDTO> CreateDestinationAsync(DestinationDTO DestinationDto)
        {
            try
            {
                ValidateDestination(DestinationDto);

                var destination = new Destination
                {
                    Name = DestinationDto.Name,
                    Description = DestinationDto.Description,
                    Country = DestinationDto.Country,
                    Region = DestinationDto.Region,
                    Latitude = DestinationDto.Latitude,
                    Longitude = DestinationDto.Longitude
                };

                var destinationCreado = await _destinationData.CreateAsync(destination);

                return new DestinationDTO
                {
                    DestinationId = destinationCreado.DestinationId,
                    Name = destinationCreado.Name,
                    Description = destinationCreado.Description,
                    Country = destinationCreado.Country,
                    Region = destinationCreado.Region,
                    Latitude = destinationCreado.Latitude,
                    Longitude = destinationCreado.Longitude
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo destino: {Name}", DestinationDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateDestination(DestinationDTO DestinationDto)
        {
            if (DestinationDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto destino no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(DestinationDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un destino con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del destino es obligatorio");
            }
        }

        // Método para actualizar el destino desde un DTO
        public async Task<DestinationDTO?> UpdateDestinationAsync(DestinationDTO destinationDto)
        {
            try
            {
                ValidateDestination(destinationDto);

                var destinationExistente = await _destinationData.GetByIdAsync(destinationDto.DestinationId);

                if (destinationExistente == null)
                {
                    return null; // El controlador se encarga de devolver NotFound
                }

                destinationExistente.Name = destinationDto.Name;
                destinationExistente.Description = destinationDto.Description;
                destinationExistente.Country = destinationDto.Country;
                destinationExistente.Region = destinationDto.Region;
                destinationExistente.Latitude = destinationDto.Latitude;
                destinationExistente.Longitude = destinationDto.Longitude;


                var destinationActualizado = await _destinationData.UpdateDestinationAsync(destinationExistente);

                return new DestinationDTO
                {
                    DestinationId = destinationActualizado.DestinationId,
                    Name = destinationActualizado.Name,
                    Description = destinationActualizado.Description,
                    Country = destinationActualizado.Country,
                    Region = destinationActualizado.Region,
                    Latitude = destinationActualizado.Latitude,
                    Longitude = destinationActualizado.Longitude
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar destino: {DestinationId}", destinationDto?.DestinationId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el destino", ex);
            }
        }

        // Método para eliminar un destino desde el DTO
        public async Task<bool> DeleteDestinationAsync(int id)
        {
            try
            {
                var destinationExistente = await _destinationData.GetByIdAsync(id);

                if (destinationExistente == null)
                {
                    return false; 
                }

                var result = await _destinationData.DeleteAsync(id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar destino con ID: {DestinationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el destino con ID {id}", ex);
            }
        }

        // Elimina lógicamente un destino (soft delete).
        public async Task<bool> SoftDeleteDestinationAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un destino con ID inválido: {DestinationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del destino debe ser mayor que cero");
            }

            try
            {
                var result = await _destinationData.SoftDeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el eliminado lógico del destino con ID: {DestinationId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar lógicamente el destino", ex);
            }
        }

        // Restaura un destino eliminado lógicamente.
        public async Task<bool> RestoreDestinationAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó restaurar un destino con ID inválido: {DestinationId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var result = await _destinationData.RestoreAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el destino con ID: {DestinationId}", id);
                throw new ExternalServiceException("Base de datos", "Error al restaurar el destino", ex);
            }
        }

        //Método para actualizar parcialmente los campos de un destino.

        public async Task<DestinationDTO?> PartialUpdateDestinationAsync(int id, DestinationDTO destinationDto)
        {
            try
            {
                var existingDestination = await _destinationData.GetByIdAsync(id);

                if (existingDestination == null)
                {
                    return null;
                }

                // Solo actualizamos los campos que vienen no nulos o no vacíos
                if (!string.IsNullOrWhiteSpace(destinationDto.Name))
                    existingDestination.Name = destinationDto.Name;

                if (!string.IsNullOrWhiteSpace(destinationDto.Description))
                    existingDestination.Description = destinationDto.Description;

                if (!string.IsNullOrWhiteSpace(destinationDto.Country))
                    existingDestination.Country = destinationDto.Country;

                if (!string.IsNullOrWhiteSpace(destinationDto.Region))
                    existingDestination.Region = destinationDto.Region;

                if (destinationDto.Latitude > 0)
                    existingDestination.Latitude = destinationDto.Latitude;

                if (destinationDto.Longitude < 0)
                    existingDestination.Longitude = destinationDto.Longitude;

                if (destinationDto.Longitude > 0)
                    existingDestination.Longitude = destinationDto.Longitude;


                var updatedDestination = await _destinationData.UpdateDestinationAsync(existingDestination);

                return new DestinationDTO
                {
                    DestinationId = updatedDestination.DestinationId,
                    Name = updatedDestination.Name,
                    Description = updatedDestination.Description,
                    Country = updatedDestination.Country,
                    Region = updatedDestination.Region,
                    Latitude = updatedDestination.Latitude,
                    Longitude = updatedDestination.Longitude
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar parcialmente el destino con ID: {DestinationId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al actualizar parcialmente el destino con ID {id}", ex);
            }
        }
    }
}