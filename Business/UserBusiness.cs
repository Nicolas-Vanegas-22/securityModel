using Data;
using Entity.DTO;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class UserBusiness
    {
        private readonly UserData _userData;
        private readonly ILogger<User> _logger;

        public UserBusiness(UserData userData, ILogger<User> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        // Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsync();
                var usersDTO = new List<UserDTO>();

                foreach (var user in users)
                {
                    usersDTO.Add(new UserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        Password = user.Password
                    });
                }

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        //Obtiene únicamente los usuarios que no están eliminados lógicamente.
        public async Task<IEnumerable<UserDTO>> GetAllActiveUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllActiveAsync();
                var usersDTO = new List<UserDTO>();

                foreach (var user in users)
                {
                    usersDTO.Add(new UserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        Email = user.Email,
                        Password = user.Password
                    });
                }

                return usersDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios activos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar los usuarios activos", ex);
            }
        }


        // Método para obtener un usuario por ID como DTO
        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("Usuario", id);
                }

                return new UserDTO
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Password = user.Password
                };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID {id}", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<UserDTO> CreateUserAsync(UserDTO UserDto)
        {
            try
            {
                ValidateUser(UserDto);

                var user = new User
                {
                    Username = UserDto.Username,
                    Email = UserDto.Email,
                    Password = UserDto.Password
                };

                user.RegistrationDate = DateTime.Now;

                var userCreado = await _userData.CreateAsync(user);

                return new UserDTO
                {
                    UserId = userCreado.UserId,
                    Username = userCreado.Username,
                    Email= userCreado.Email,
                    Password = userCreado.Password
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo usuario: {Username}", UserDto?.Username ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el usuario", ex);
            }
        }

        // Método para validar el DTO
        private void ValidateUser(UserDTO UserDto)
        {
            if (UserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(UserDto.Username))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del usuario es obligatorio");
            }
        }

        // Método para actualizar el usuario desde un DTO
        public async Task<UserDTO?> UpdateUserAsync(UserDTO UserDto)
        {
            try
            {
                ValidateUser(UserDto);

                var userExistente = await _userData.GetByIdAsync(UserDto.UserId);

                if (userExistente == null)
                {
                    return null; // El controlador se encarga de devolver NotFound
                }

                userExistente.Username = UserDto.Username;
                userExistente.Email = UserDto.Email;
                userExistente.Password = UserDto.Password;

                var userActualizado = await _userData.UpdateUserAsync(userExistente);

                return new UserDTO
                {
                    UserId = userActualizado.UserId,
                    Username = userActualizado.Username,
                    Email = userActualizado.Email,
                    Password = userActualizado.Password
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario: {UserId}", UserDto?.UserId ?? 0);
                throw new ExternalServiceException("Base de datos", "Error al actualizar el usuario", ex);
            }
        }

        // Método para eliminar un usuario por ID
        public async Task<bool> DeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogInformation("No se encontró ningún usuario con ID: {UserId}", id);
                    throw new EntityNotFoundException("Usuario", id);
                }

                var deleted = await _userData.DeleteAsync(id);

                if (!deleted)
                {
                    _logger.LogError("Error al eliminar el usuario con ID: {UserId}", id);
                    throw new ExternalServiceException("Base de datos", $"No se pudo eliminar el usuario con ID {id}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al eliminar el usuario con ID {id}", ex);
            }
        }

        // Elimina lógicamente un usuario (soft delete).
        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó eliminar lógicamente un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var result = await _userData.SoftDeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar el eliminado lógico del usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar lógicamente el usuario", ex);
            }
        }

        // Restaura un usuario eliminado lógicamente.
        public async Task<bool> RestoreUserAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó restaurar un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var result = await _userData.RestoreAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al restaurar el usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al restaurar el usuario", ex);
            }
        }

        // Modifica un usuario parcialmente.
        public async Task<UserDTO?> PartialUpdateUserAsync(int id, PartialUserDTO partialUserDto)
        {
            try
            {
                var userExistente = await _userData.GetByIdAsync(id);

                if (userExistente == null)
                {
                    return null; // No se encontró el usuario
                }

                // Modificar solo los campos presentes en el DTO
                if (!string.IsNullOrEmpty(partialUserDto.Username))
                {
                    userExistente.Username = partialUserDto.Username;
                }

                if (!string.IsNullOrEmpty(partialUserDto.Email))
                {
                    userExistente.Email = partialUserDto.Email;
                }

                if (!string.IsNullOrEmpty(partialUserDto.Password))
                {
                    userExistente.Password = partialUserDto.Password;
                }

                // Aquí actualizamos los datos en la base de datos
                var updatedUser = await _userData.UpdateUserAsync(userExistente);

                return new UserDTO
                {
                    UserId = updatedUser.UserId,
                    Username = updatedUser.Username,
                    Email = updatedUser.Email,
                    Password = updatedUser.Password
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar parte del usuario con ID: {UserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al modificar parte del usuario", ex);
            }
        }

    }
}