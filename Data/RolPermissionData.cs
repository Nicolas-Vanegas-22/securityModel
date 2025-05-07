using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad Rol Permiso en la base de datos.
    /// </summary>
    public class RolPermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolPermission> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public RolPermissionData(ApplicationDbContext context, ILogger<RolPermission> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los rol permiso almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de usuarios.</returns>
        public async Task<IEnumerable<RolPermission>> GetAllAsync()
        {
            return await _context.Set<RolPermission>().ToListAsync();
        }

        public async Task<RolPermission?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<RolPermission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol permiso con ID {RolPermissionId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol permiso en la base de datos.
        ///</summary>
        ///<param name="rolPermission">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<RolPermission> CreateAsync(RolPermission rolPermission)
        {
            try
            {
                await _context.Set<RolPermission>().AddAsync(rolPermission);
                await _context.SaveChangesAsync();
                return rolPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el rol permiso: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol permiso existente en la base de datos.
        ///</summary>
        ///<param name="rolPermission">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(RolPermission rolPermission)
        {
            try
            {
                _context.Set<RolPermission>().Update(rolPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el rol permiso: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un rol permiso de la base de datos.
        ///</summary>
        ///<param name="id">Identificador único del rol permiso a eliminar.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolPermission = await _context.Set<RolPermission>().FindAsync(id);
                if (rolPermission == null)
                    return false;

                _context.Set<RolPermission>().Remove(rolPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el rol permiso: {ex.Message}");
                return false;
            }
        }
    }
}