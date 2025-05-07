using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Usuario en la base de datos.
    /// </summary>
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<User> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public UserData(ApplicationDbContext context, ILogger<User> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los usuarios almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de usuarios.</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Set<User>().ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {UserId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        /// <summary>
        /// Obtiene únicamente los usuarios que no están eliminados lógicamente.
        /// </summary>
        /// <returns>Lista de usuarios activos.</returns>
        public async Task<IEnumerable<User>> GetAllActiveAsync()
        {
            return await _context.Set<User>()
                                 .Where(u => u.DeleteAt == null)
                                 .ToListAsync();
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="user">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol existente en la base de datos.
        ///</summary>
        ///<param name="user">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un rol de la base de datos.
        ///</summary>
        ///<param name="id">Identificador único del rol a eliminar.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Actualiza un usuario existente en la base de datos.
        /// </summary>
        /// <param name="user">Instancia del usuario con datos actualizados.</param>
        /// <returns>El usuario actualizado.</returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Realiza un eliminado lógico del usuario (marca el campo DeleteAt).
        /// </summary>
        /// <param name="id">ID del usuario a eliminar lógicamente.</param>
        /// <returns>True si la operación fue exitosa, False si no se encontró el usuario.</returns>
        public async Task<bool> SoftDeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                user.DeleteAt = DateTime.UtcNow;
                _context.Set<User>().Update(user);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el eliminado lógico del usuario con ID {UserId}", id);
                return false;
            }
        }


        /// <summary>
        /// Restaura un usuario eliminado lógicamente (pone DeleteAt en null).
        /// </summary>
        /// <param name="id">ID del usuario a restaurar.</param>
        /// <returns>True si la operación fue exitosa, False si no se encontró el usuario.</returns>
        public async Task<bool> RestoreAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);
                if (user == null)
                    return false;

                user.DeleteAt = null;
                _context.Set<User>().Update(user);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el usuario con ID {UserId}", id);
                return false;
            }
        }

    }
}