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
    /// Controlador para la gestión de pagos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentBusiness _PaymentBusiness;
        private readonly ILogger<PaymentController> _logger;

        /// <summary>
        /// Constructor del controlador de pagos
        /// </summary>
        public PaymentController(PaymentBusiness PaymentBusiness, ILogger<PaymentController> logger)
        {
            _PaymentBusiness = PaymentBusiness;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pagos del sistema
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentDTO>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var payments = await _PaymentBusiness.GetAllPaymentsAsync();
                return Ok(payments);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener pagos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un pago específico por su ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPaymentById(int id)
        {
            try
            {
                var payment = await _PaymentBusiness.GetPaymentByIdAsync(id);
                return Ok(payment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para pago con ID: {PaymentId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Pago no encontrado con ID: {PaymentId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener pago con ID: {PaymentId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo pago en el sistema
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePerson([FromBody] PaymentDTO PaymentDto)
        {
            try
            {
                var createdPayment = await _PaymentBusiness.CreatePaymentAsync(PaymentDto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.PaymentId }, createdPayment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear pago");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear pago");
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}