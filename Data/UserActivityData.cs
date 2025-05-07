using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Usuario Actividad en la base de datos.
    /// </summary>
    public class UserActivityData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserActivity> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public UserActivityData(ApplicationDbContext context, ILogger<UserActivity> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los usuarios actividad almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de usuarios actividad.</returns>
        public async Task<IEnumerable<UserActivity>> GetAllAsync()
        {
            return await _context.Set<UserActivity>().ToListAsync();
        }

        public async Task<UserActivity?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserActivity>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {UserActivityId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo usuario actividad en la base de datos.
        ///</summary>
        ///<param name="userRol">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<UserActivity> CreateAsync(UserActivity userRol)
        {
            try
            {
                await _context.Set<UserActivity>().AddAsync(userRol);
                await _context.SaveChangesAsync();
                return userRol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario actividad: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un usuario actividad existente en la base de datos.
        ///</summary>
        ///<param name="userRol">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(UserActivity userRol)
        {
            try
            {
                _context.Set<UserActivity>().Update(userRol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario actividad: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un usuario actividad de la base de datos.
        ///</summary>
        ///<param name="id">Identificador único del rol a eliminar.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var userRol = await _context.Set<UserActivity>().FindAsync(id);
                if (userRol == null)
                    return false;

                _context.Set<UserActivity>().Remove(userRol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario actividad: {ex.Message}");
                return false;
            }
        }
    }
}