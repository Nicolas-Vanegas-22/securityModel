using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{

    public class PaymentBusiness
    {
        private readonly PaymentData _paymentData;
        private readonly ILogger<Payment> _logger;

        public PaymentBusiness(PaymentData paymentData, ILogger<Payment> logger)
        {
            _paymentData = paymentData;
            _logger = logger;
        }

        // Método para obtener todos los pagos como DTOs
        public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
        {
            try
            {
                var payments = await _paymentData.GetAllAsync();
                var paymentsDTO = new List<PaymentDTO>();

                foreach (var payment in payments)
                {
                    paymentsDTO.Add(new PaymentDTO
                    {
                        PaymentId = payment.PaymentId,
                        PaymentMethod = payment.PaymentMethod,
                        Amount = payment.Amount,
                        Activity = payment.Activity
                    });
                }

                return paymentsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los pagos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de pagos", ex);
            }
        }

        // Método para obtener un pago por ID como DTO
        public async Task<PaymentDTO> GetPaymentByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un pago con ID inválido: {PaymentId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del pago debe ser mayor que cero");
            }

            try
            {
                var payment = await _paymentData.GetByIdAsync(id);
                if (payment == null)
                {
                    _logger.LogInformation("No se encontró ningún pago con ID: {PaymentId}", id);
                    throw new EntityNotFoundException("Pago", id);
                }

                return new PaymentDTO
                {
                    PaymentMethod = payment.PaymentMethod,
                    Amount = payment.Amount,
                    Activity = payment.Activity
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el pago con ID: {PaymentId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el pago con ID {id}", ex);
            }
        }

        // Método para crear un pago desde un DTO
        public async Task<PaymentDTO> CreatePaymentAsync(PaymentDTO PaymentDto)
        {
            try
            {
                ValidatePayment(PaymentDto);

                var payment = new Payment
                {
                    PaymentMethod = PaymentDto.PaymentMethod,
                    Amount = PaymentDto.Amount,
                    Activity = PaymentDto.Activity
                };

                var paymentCreado = await _paymentData.CreateAsync(payment);

                return new PaymentDTO
                {
                    PaymentId = paymentCreado.PaymentId,
                    PaymentMethod = paymentCreado.PaymentMethod,
                    Amount = paymentCreado.Amount,
                    Activity = paymentCreado.Activity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo pago: {Activity}", PaymentDto?.Activity ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el pago", ex);
            }
        }

        // Método para validar el DTO
        private void ValidatePayment(PaymentDTO PaymentDto)
        {
            if (PaymentDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto pago no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(PaymentDto.Activity))
            {
                _logger.LogWarning("Se intentó crear/actualizar un pago con Activity vacío");
                throw new Utilities.Exceptions.ValidationException("Pago", "La Activity del pago es obligatorio");
            }
        }
    }
}