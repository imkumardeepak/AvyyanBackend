using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(int userId);

        // Registration
        Task<UserDto> RegisterAsync(RegisterDto registerDto);

        // Password Management
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto);

        // Token Management
        string GenerateJwtToken(UserDto user, IEnumerable<string> roles);

        // Authentication Helpers
        Task<bool> ValidatePasswordAsync(string password, string hash);
        string HashPassword(string password);
    }
}