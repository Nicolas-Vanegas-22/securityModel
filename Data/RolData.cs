using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Entity.DTO;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Rol en la base de datos.
    /// </summary>
    public class RolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Rol> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public RolData(ApplicationDbContext context, ILogger<Rol> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los roles almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de roles.</returns>
        public async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Set<Rol>().ToListAsync();
        }

        public async Task<Rol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Rol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol con ID {RolId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="rol">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<Rol> CreateAsync(Rol rol)
        {
            try
            {
                await _context.Set<Rol>().AddAsync(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erorr al crear el rol: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol existente en la base de datos.
        ///</summary>
        ///<param name="rol">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Realiza un eliminado lógico del rol (marca el campo DeleteAt).
        /// </summary>
        /// <param name="id">ID del usuario a eliminar lógicamente.</param>
        /// <returns>True si la operación fue exitosa, False si no se encontró el usuario.</returns>
        public async Task<bool> SoftDeleteAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                rol.DeleteAt = DateTime.UtcNow;
                _context.Set<Rol>().Update(rol);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el eliminado lógico del rol con ID {RolId}", id);
                return false;
            }
        }


        /// <summary>
        /// Elimina un rol de la base de datos.
        /// </summary>
        /// <param name="id">Identificador único del rol a eliminar.</param>
        /// <returns>True si la operación fue exitosa, False si no se encontró el rol.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                {
                    return false; // El rol no existe en la base de datos
                }

                _context.Set<Rol>().Remove(rol);
                await _context.SaveChangesAsync();
                return true; // Rol eliminado correctamente
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el rol con ID {RolId}", id);
                throw; // Relanzamos la excepción para ser manejada por el Business
            }
        }


        /// <summary>
        /// Actualiza un usuario existente en la base de datos.
        /// </summary>
        /// <param name="rol">Instancia del rol con datos actualizados.</param>
        /// <returns>El rol actualizado.</returns>
        public async Task<Rol> UpdateRolAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Restaura un rol eliminado lógicamente (pone DeleteAt en null).
        /// </summary>
        /// <param name="id">ID del rol a restaurar.</param>
        /// <returns>True si la operación fue exitosa, False si no se encontró el rol.</returns>
        public async Task<bool> RestoreAsync(int id)
        {
            try
            {
                var rol = await _context.Set<Rol>().FindAsync(id);
                if (rol == null)
                    return false;

                rol.DeleteAt = null;
                _context.Set<Rol>().Update(rol);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el rol con ID {UserId}", id);
                return false;
            }
        }

        /// <summary>
        /// Actualiza un rol existente en la base de datos.
        /// </summary>
        /// <param name="rol">Instancia del rol con datos actualizados.</param>
        /// <returns>El usuario actualizado.</returns>
        public async Task<Rol> UpdateUserAsync(Rol rol)
        {
            try
            {
                _context.Set<Rol>().Update(rol);
                await _context.SaveChangesAsync();
                return rol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol: {ex.Message}");
                throw;
            }
        }


    }
}