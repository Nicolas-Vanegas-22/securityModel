using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gesti�n de la entidad Rol en la base de datos.
    /// </summary>
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<Module> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexi�n con la base de datos.</param>
        public ModuleData(ApplicationDbContext context, ILogger<Module> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los roles almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de roles.</returns>
        public async Task<IEnumerable<Module>> GetAllAsync()
        {
            return await _context.Set<Module>().ToListAsync();
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener modulo con ID {ModuleId}", id);
                throw; //Re-lanza la excepci�n para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="module">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear modulo: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un rol existente en la base de datos.
        ///</summary>
        ///<param name="module">Objeto con la informaci�n actualizada.</param>
        ///<returns>True si la operaci�n fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar modulo: {ex.Message}");
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
                var module = await _context.Set<Module>().FindAsync(id);
                if (module == null)
                    return false;

                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar modulo: {ex.Message}");
                return false;
            }
        }
    }
}