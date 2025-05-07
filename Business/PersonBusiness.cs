using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<Person> _logger;

        public PersonBusiness(PersonData personData, ILogger<Person> logger)
        {
            _personData = personData;
            _logger = logger;
        }

        // Método para obtener todas las personas como DTOs
        public async Task<IEnumerable<PersonDTO>> GetAllPersonsAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                var personsDTO = new List<PersonDTO>();

                foreach (var person in persons)
                {
                    personsDTO.Add(new PersonDTO
                    {
                        PersonId = person.PersonId,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        Document = person.Document,
                        PhoneNumber = person.PhoneNumber,
                        Email = person.Email
                    });
                }

                return personsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);
            }
        }

        // Método para obtener una persona por ID como DTO
        public async Task<PersonDTO> GetPersonByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválido: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de la persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ningúna persona con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return new PersonDTO
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Document = person.Document,
                    PhoneNumber = person.PhoneNumber,
                    Email = person.Email
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar la persona con ID {id}", ex);
            }
        }

        // Método para crear una persona desde un DTO
        public async Task<PersonDTO> CreatePersonAsync(PersonDTO PersonDto)
        {
            try
            {
                ValidatePerson(PersonDto);

                var person = new Person
                {
                    FirstName = PersonDto.FirstName,
                    LastName = PersonDto.LastName,
                    Document = PersonDto.Document,
                    PhoneNumber = PersonDto.PhoneNumber,
                    Email = PersonDto.Email
                };

                var personCreado = await _personData.CreateAsync(person);

                return new PersonDTO
                {
                    PersonId = personCreado.PersonId,
                    FirstName = personCreado.FirstName,
                    LastName = personCreado.LastName,
                    Document = personCreado.Document,
                    PhoneNumber = personCreado.PhoneNumber,
                    Email = personCreado.Email
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva persona: {RolNombre}", PersonDto?.FirstName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear la persona", ex);
            }
        }

        // Método para validar el DTO
        private void ValidatePerson(PersonDTO PersonDto)
        {
            if (PersonDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PersonDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de la persona es obligatorio");
            }
        }

    }
}