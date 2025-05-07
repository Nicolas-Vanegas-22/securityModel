using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{

    public class FormBusiness
    {
        private readonly FormData _formData;
        private readonly ILogger<Form> _logger;

        public FormBusiness(FormData formData, ILogger<Form> logger)
        {
            _formData = formData;
            _logger = logger;
        }

        // M�todo para obtener todos los formularios como DTOs
        public async Task<IEnumerable<FormDTO>> GetAllFormsAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                var formsDTO = new List<FormDTO>();

                foreach (var form in forms)
                {
                    formsDTO.Add(new FormDTO
                    {
                        FormId = form.FormId,
                        Name = form.Name
                    });
                }

                return formsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        // M�todo para obtener un formulario por ID como DTO
        public async Task<FormDTO> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intent� obtener un formulario con ID inv�lido: {FormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del formulario debe ser mayor que cero");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogInformation("No se encontr� ning�n formulario con ID: {FormId}", id);
                    throw new EntityNotFoundException("Form", id);
                }

                return new FormDTO
                {
                    FormId = form.FormId,
                    Name = form.Name,
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formulario con ID: {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el formulario con ID {id}", ex);
            }
        }

        // M�todo para crear un formulario desde un DTO
        public async Task<FormDTO> CreateFormAsync(FormDTO FormDto)
        {
            try
            {
                ValidateForm(FormDto);

                var form = new Form
                {
                    Name = FormDto.Name,
                };

                var formCreado = await _formData.CreateAsync(form);

                return new FormDTO
                {
                    FormId = formCreado.FormId,
                    Name = formCreado.Name,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo formulario: {Name}", FormDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el formulario", ex);
            }
        }

        // M�todo para validar el DTO
        private void ValidateForm(FormDTO FormDto)
        {
            if (FormDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto formulario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(FormDto.Name))
            {
                _logger.LogWarning("Se intent� crear/actualizar un formulario con Name vac�o");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del formulario es obligatorio");
            }
        }
    }
}