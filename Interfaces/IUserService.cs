using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IUserService
    {
        // User Management
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);

        // Profile Management
        Task<UserDto?> UpdateProfileAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

        // User Permissions
        Task<IEnumerable<PageAccessDto>> GetUserPageAccessesAsync(int userId);
        Task<bool> HasPageAccessAsync(int userId, string pageName);

        // Validation
        Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
        Task<bool> ValidatePasswordAsync(string password);
    }
}