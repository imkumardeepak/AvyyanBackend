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
        private readonly IRepository<RoleMaster> _roleRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUnitOfWork unitOfWork,
            IRepository<User> userRepository,
            IRepository<RoleMaster> roleRepository,
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
            _logger.LogDebug("Attempting login for email: {Email}", loginDto.Email);

            var user = await _userService.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found for email: {Email}", loginDto.Email);
                return null;
            }

            var userEntity = await _userRepository.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (userEntity == null) return null;

            if (!await ValidatePasswordAsync(loginDto.Password, userEntity.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for email: {Email}", loginDto.Email);
                return null;
            }

            userEntity.LastLoginAt = DateTime.Now;
            _userRepository.Update(userEntity);
            await _unitOfWork.SaveChangesAsync();

            var pageAccesses = await _userService.GetUserPageAccessesAsync(user.Id);

            var roles = new List<string> { user.RoleName };
            var token = GenerateJwtToken(user, roles);

            _logger.LogInformation("User {Email} logged in successfully", loginDto.Email);

            return new LoginResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.Now.AddHours(1),
                User = user,
                Roles = roles,
                PageAccesses = pageAccesses
            };
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            // Nothing to do here anymore as we don't handle refresh tokens
            return await Task.FromResult(true);
        }

        public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
        {
            if (!await _userService.IsEmailUniqueAsync(registerDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                RoleName = "User" // Assign a default role
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

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

            _logger.LogInformation("Password reset requested for user: {Email}", resetPasswordDto.Email);
            return true;
        }

        public Task<bool> SetPasswordAsync(SetPasswordDto setPasswordDto)
        {
            return Task.FromResult(true);
        }

        public string GenerateJwtToken(UserDto user, IEnumerable<string> roles)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? "YourSecretKeyHere");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new("FullName", $"{user.FirstName} {user.LastName}")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
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