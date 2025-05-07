using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad Rol en la base de datos.
    /// </summary>
    public class PaymentData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Payment> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        public PaymentData(ApplicationDbContext context, ILogger<Payment> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los roles almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de roles.</returns>
        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            return await _context.Set<Payment>().ToListAsync();
        }

        public async Task<Payment?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Payment>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener pago con ID {PaymentId}", id);
                throw; //Re-lanza la excepci�n para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="payment">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<Payment> CreateAsync(Payment payment)
        {
            try
            {
                await _context.Set<Payment>().AddAsync(payment);
                await _context.SaveChangesAsync();
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear pago: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol existente en la base de datos.
        ///</summary>
        ///<param name="payment">Objeto con la informaci�n actualizada.</param>
        ///<returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Payment payment)
        {
            try
            {
                _context.Set<Payment>().Update(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar pago: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un rol de la base de datos.
        ///</summary>
        ///<param name="id">Identificador �nico del rol a eliminar.</param>
        ///<returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var payment = await _context.Set<Payment>().FindAsync(id);
                if (payment == null)
                    return false;

                _context.Set<Payment>().Remove(payment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar pago: {ex.Message}");
                return false;
            }
        }
    }
}