using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<RoleMaster> _roleRepository;
        private readonly IRepository<PageAccess> _pageAccessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IRepository<User> userRepository,
            IRepository<RoleMaster> roleRepository,
            IRepository<PageAccess> pageAccessRepository,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _pageAccessRepository = pageAccessRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (!await IsEmailUniqueAsync(createUserDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                PhoneNumber = createUserDto.PhoneNumber,
                RoleName = createUserDto.RoleName
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return null;

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return null;

            if (user.Email != updateUserDto.Email && !await IsEmailUniqueAsync(updateUserDto.Email, userId))
                throw new InvalidOperationException("Email already exists");

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.IsActive = updateUserDto.IsActive;
            user.UpdatedAt = DateTime.Now;
            user.RoleName = updateUserDto.RoleName;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<UserDto?> UpdateProfileAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return null;

            if (user.Email != updateUserDto.Email && !await IsEmailUniqueAsync(updateUserDto.Email, userId))
                throw new InvalidOperationException("Email already exists");

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.Email = updateUserDto.Email;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.UpdatedAt = DateTime.Now;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;
            _userRepository.Remove(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.Now;

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PageAccessDto>> GetUserPageAccessesAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return new List<PageAccessDto>();

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.RoleName == user.RoleName);
            if (role == null) return new List<PageAccessDto>();

            var pageAccesses = await _pageAccessRepository.FindAsync(pa =>
                pa.RoleId == role.Id);

            return _mapper.Map<IEnumerable<PageAccessDto>>(pageAccesses.OrderBy(pa => pa.PageName));
        }

        public async Task<bool> HasPageAccessAsync(int userId, string pageName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var role = await _roleRepository.FirstOrDefaultAsync(r => r.RoleName == user.RoleName);
            if (role == null) return false;

            var pageAccess = await _pageAccessRepository.FirstOrDefaultAsync(pa => pa.PageName == pageName);

            if (pageAccess == null) return false;

            return role.Id == pageAccess.RoleId;
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeUserId = null)
        {
            var query = await _userRepository.FindAsync(u => u.Email == email);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            return !query.Any();
        }

        public Task<bool> ValidatePasswordAsync(string password)
        {
            // Basic password validation
            return Task.FromResult(!string.IsNullOrEmpty(password) && password.Length >= 6);
        }
    }
}