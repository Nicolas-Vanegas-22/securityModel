﻿using Microsoft.Extensions.Logging;
using Entity.Model;
using Entity.Context;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    /// <summary>
    /// Repositorio encargado de la gestión de la entidad UserRol en la base de datos.
    /// </summary>
    public class UserRolData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRol> _logger;

        ///<summary>
        ///Constructor que recibe el contexto de base de datos.
        ///</summary>
        ///<param name="=context">Instancia de <see cref="ApplicationDbContext"/> para la conexión con la base de datos.</param>
        public UserRolData(ApplicationDbContext context, ILogger<UserRol> logger)
        {
            _context = context;
            _logger = logger;
        }

        ///<summary>
        ///Obtiene todos los roles almacenados en la base de datos.
        ///</summary>
        ///<returns>Lista de roles.</returns>
        public async Task<IEnumerable<UserRol>> GetAllAsync()
        {
            return await _context.Set<UserRol>().ToListAsync();
        }

        public async Task<UserRol?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<UserRol>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol usuario con ID {UserRolId}", id);
                throw; //Re-lanza la excepción para que sea manejada en capas superiores
            }
        }

        ///<summary>
        ///Crea un nuevo rol en la base de datos.
        ///</summary>
        ///<param name="userRol">Instancia del rol a crear.</param>
        ///<returns>El rol creado.</returns>
        public async Task<UserRol> CreateAsync(UserRol userRol)
        {
            try
            {
                await _context.Set<UserRol>().AddAsync(userRol);
                await _context.SaveChangesAsync();
                return userRol;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el usuario rol: {ex.Message}");
                throw;
            }
        }

        ///<summary>
        ///Actualiza un usuario rol existente en la base de datos.
        ///</summary>
        ///<param name="userRol">Objeto con la información actualizada.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> UpdateAsync(UserRol userRol)
        {
            try
            {
                _context.Set<UserRol>().Update(userRol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el usuario rol: {ex.Message}");
                return false;
            }
        }

        ///<summary>
        ///Elimina un usuario rol de la base de datos.
        ///</summary>
        ///<param name="id">Identificador único del usuario rol a eliminar.</param>
        ///<returns>True si la operación fue exitosa, False en caso contrario.</returns>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var userRol = await _context.Set<UserRol>().FindAsync(id);
                if (userRol == null)
                    return false;

                _context.Set<UserRol>().Remove(userRol);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el usuario rol: {ex.Message}");
                return false;
            }
        }
    }
}