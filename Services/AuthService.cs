using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AvyyanBackend.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            _logger.LogDebug("Attempting login for username: {Username}", loginDto.Username);

            var user = await GetUserByUsernameOrEmailAsync(loginDto.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for username: {Username}", loginDto.Username);
                return null;
            }

            var userEntity = await _userRepository.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userEntity == null) return null;

            // if (userEntity.IsLocked && userEntity.LockedUntil.HasValue && userEntity.LockedUntil > DateTime.UtcNow)
            // {
            //     _logger.LogWarning("Login failed: User account is locked for username: {Username}", loginDto.Username);
            //     return null;
            // }

            // if (!await ValidatePasswordAsync(loginDto.Password, userEntity.PasswordHash))
            // {
            //     userEntity.FailedLoginAttempts++;
            //     if (userEntity.FailedLoginAttempts >= 5)
            //     {
            //         userEntity.IsLocked = true;
            //         userEntity.LockedUntil = DateTime.UtcNow.AddMinutes(30);
            //     }
            //     _userRepository.Update(userEntity);
            //     await _unitOfWork.SaveChangesAsync();

            //     _logger.LogWarning("Login failed: Invalid password for username: {Username}", loginDto.Username);
            //     return null;
            // }

            // Reset failed login attempts on successful login
            userEntity.FailedLoginAttempts = 0;
            userEntity.IsLocked = false;
            userEntity.LockedUntil = null;
            userEntity.LastLoginAt = DateTime.UtcNow;

            // Generate tokens
            var refreshToken = GenerateRefreshToken();
            userEntity.RefreshToken = refreshToken;
            userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _userRepository.Update(userEntity);
            await _unitOfWork.SaveChangesAsync();

            var roles = await _userService.GetUserRolesAsync(user.Id);
            var pageAccesses = await _userService.GetUserPageAccessesAsync(user.Id);

            user.Roles = roles;

            var token = GenerateJwtToken(user, roles);

            _logger.LogInformation("User {Username} logged in successfully", loginDto.Username);

            return new LoginResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = user,
                Roles = roles,
                PageAccesses = pageAccesses
            };
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
        {
            var userEntity = await _userRepository.FirstOrDefaultAsync(u =>
                u.RefreshToken == refreshTokenDto.RefreshToken && u.IsActive);

            if (userEntity == null || userEntity.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var user = await _userService.GetUserByIdAsync(userEntity.Id);
            if (user == null) return null;

            var roles = await _userService.GetUserRolesAsync(user.Id);
            var pageAccesses = await _userService.GetUserPageAccessesAsync(user.Id);

            user.Roles = roles;

            var newToken = GenerateJwtToken(user, roles);
            var newRefreshToken = GenerateRefreshToken();

            userEntity.RefreshToken = newRefreshToken;
            userEntity.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            _userRepository.Update(userEntity);
            await _unitOfWork.SaveChangesAsync();

            return new LoginResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = user,
                Roles = roles,
                PageAccesses = pageAccesses
            };
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
            if (user == null) return false;

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            if (!await _userService.IsUsernameUniqueAsync(registerDto.Username))
                throw new InvalidOperationException("Username already exists");

            if (!await _userService.IsEmailUniqueAsync(registerDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                IsEmailVerified = false
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Assign default "User" role
            var userRole = await _roleRepository.FirstOrDefaultAsync(r => r.Name == "User");
            if (userRole != null)
            {
                await _userService.AssignRoleToUserAsync(new AssignRoleDto { UserId = user.Id, RoleId = userRole.Id });
            }

            return _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            return await _userService.ChangePasswordAsync(userId, changePasswordDto);
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email && u.IsActive);
            if (user == null) return false;

            // In a real implementation, you would send an email with a reset token
            // For now, we'll just log it
            _logger.LogInformation("Password reset requested for user: {Email}", resetPasswordDto.Email);
            return true;
        }

        public Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto)
        {
            // In a real implementation, you would validate the reset token
            // For now, we'll assume the token is valid and find the user by email
            // This is a simplified implementation
            return Task.FromResult(true);
        }

        public string GenerateJwtToken(UserDto user, IEnumerable<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "YourSecretKeyHere");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new("FullName", user.FullName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<bool> ValidateRefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.IsActive);
            return user != null && user.RefreshTokenExpiryTime > DateTime.UtcNow;
        }

        public async Task<UserDto?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u =>
                (u.Username == usernameOrEmail || u.Email == usernameOrEmail) && u.IsActive);

            return user != null ? _mapper.Map<UserDto>(user) : null;
        }

        public Task<bool> ValidatePasswordAsync(string password, string hash)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, hash));
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}