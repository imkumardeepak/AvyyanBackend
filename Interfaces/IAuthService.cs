using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IAuthService
    {
        // Authentication
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
        Task<bool> LogoutAsync(int userId);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);

        // Registration
        Task<UserDto> RegisterAsync(RegisterDto registerDto);

        // Password Management
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto);

        // Token Management
        string GenerateJwtToken(UserDto user, IEnumerable<string> roles);
        string GenerateRefreshToken();
        Task<bool> ValidateRefreshTokenAsync(string refreshToken);

        // Authentication Helpers
        Task<UserDto?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<bool> ValidatePasswordAsync(string password, string hash);
        string HashPassword(string password);
    }
}
