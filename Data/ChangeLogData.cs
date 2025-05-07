using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad ChangeLog en la base de datos.
    /// </summary>
    public class ChangeLogData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ChangeLog> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public ChangeLogData(ApplicationDbContext context, ILogger<ChangeLog> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los cambios almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de cambios.</returns>
        public async Task<IEnumerable<ChangeLog>> GetAllAsync()
        {
            return await _context.Set<ChangeLog>().ToListAsync();
        }

        public async Task<ChangeLog?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<ChangeLog>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cambio con ID {ChangeLogId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo cambio en la base de datos.
        ///</summary>
        ///<param name="user">Instancia del cambio a crear.</param>
        ///<returns>El cambio creado.</returns>
        public async Task<ChangeLog> CreateAsync(ChangeLog changeLog)
        {
            try
            {
                await _context.Set<ChangeLog>().AddAsync(changeLog);
                await _context.SaveChangesAsync();
                return changeLog;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el cambio: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un cambio existente en la base de datos.
        ///</summary>
        ///<param name="changeLog">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(ChangeLog changeLog)
        {
            try
            {
                _context.Set<ChangeLog>().Update(changeLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el cambio: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un cambio de la base de datos.
        ///</summary>
        ///<param name="id">Identificador único del cambio a eliminar.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var changeLog = await _context.Set<ChangeLog>().FindAsync(id);
                if (changeLog == null)
                    return false;

                _context.Set<ChangeLog>().Remove(changeLog);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el cambio: {ex.Message}");
                return false;
            }
        }
    }
}