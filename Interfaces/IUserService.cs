using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IUserService
    {
        // User Management
        Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> GetUserByUsernameAsync(string username);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> LockUserAsync(int userId);
        Task<bool> UnlockUserAsync(int userId);

        // Profile Management
        Task<UserDto?> UpdateProfileAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);

        // User Role Management
        Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);

        // User Permissions
        Task<IEnumerable<PageAccessDto>> GetUserPageAccessesAsync(int userId);
        Task<bool> HasPageAccessAsync(int userId, string pageUrl, string permission = "View");

        // Validation
        Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null);
        Task<bool> ValidatePasswordAsync(string password);
    }
}
