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
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<PageAccess> _pageAccessRepository;
        private readonly IRepository<RolePageAccess> _rolePageAccessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IRepository<User> userRepository,
            IRepository<Role> roleRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<PageAccess> pageAccessRepository,
            IRepository<RolePageAccess> rolePageAccessRepository,
            IMapper mapper,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _pageAccessRepository = pageAccessRepository;
            _rolePageAccessRepository = rolePageAccessRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            if (!await IsUsernameUniqueAsync(createUserDto.Username))
                throw new InvalidOperationException("Username already exists");

            if (!await IsEmailUniqueAsync(createUserDto.Email))
                throw new InvalidOperationException("Email already exists");

            var user = new User
            {
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password),
                PhoneNumber = createUserDto.PhoneNumber,
                IsEmailVerified = true
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Assign roles
            foreach (var roleId in createUserDto.RoleIds)
            {
                await AssignRoleToUserAsync(new AssignRoleDto { UserId = user.Id, RoleId = roleId });
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await GetUserRolesAsync(user.Id);
            return userDto;
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return null;

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await GetUserRolesAsync(userId);
            return userDto;
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await GetUserRolesAsync(user.Id);
            return userDto;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
            if (user == null) return null;

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await GetUserRolesAsync(user.Id);
            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.FindAsync(u => u.IsActive);
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserDto>(user);
                userDto.Roles = await GetUserRolesAsync(user.Id);
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
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            // Update roles
            var currentRoles = await _userRoleRepository.FindAsync(ur => ur.UserId == userId);
            foreach (var role in currentRoles)
            {
                _userRoleRepository.Remove(role);
            }

            foreach (var roleId in updateUserDto.RoleIds)
            {
                await AssignRoleToUserAsync(new AssignRoleDto { UserId = userId, RoleId = roleId });
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await GetUserRolesAsync(userId);
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
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await GetUserRolesAsync(userId);
            return userDto;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> LockUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsLocked = true;
            user.LockedUntil = DateTime.UtcNow.AddDays(30);

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UnlockUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.IsLocked = false;
            user.LockedUntil = null;
            user.FailedLoginAttempts = 0;

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive) return false;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _userRepository.Update(user);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto)
        {
            var existingUserRole = await _userRoleRepository.FirstOrDefaultAsync(ur =>
                ur.UserId == assignRoleDto.UserId && ur.RoleId == assignRoleDto.RoleId);

            if (existingUserRole != null) return true; // Already assigned

            var userRole = new UserRole
            {
                UserId = assignRoleDto.UserId,
                RoleId = assignRoleDto.RoleId,
                ExpiresAt = assignRoleDto.ExpiresAt,
                AssignedAt = DateTime.UtcNow
            };

            await _userRoleRepository.AddAsync(userRole);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var userRole = await _userRoleRepository.FirstOrDefaultAsync(ur =>
                ur.UserId == userId && ur.RoleId == roleId);

            if (userRole == null) return false;

            _userRoleRepository.Remove(userRole);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            var userRoles = await _userRoleRepository.FindAsync(ur =>
                ur.UserId == userId &&
                ur.IsActive &&
                (!ur.ExpiresAt.HasValue || ur.ExpiresAt > DateTime.UtcNow));

            var roleIds = userRoles.Select(ur => ur.RoleId);
            var roles = await _roleRepository.FindAsync(r => roleIds.Contains(r.Id) && r.IsActive);

            return roles.Select(r => r.Name);
        }

        public async Task<IEnumerable<PageAccessDto>> GetUserPageAccessesAsync(int userId)
        {
            var userRoles = await _userRoleRepository.FindAsync(ur =>
                ur.UserId == userId &&
                ur.IsActive &&
                (!ur.ExpiresAt.HasValue || ur.ExpiresAt > DateTime.UtcNow));

            var roleIds = userRoles.Select(ur => ur.RoleId);
            var rolePageAccesses = await _rolePageAccessRepository.FindAsync(rpa =>
                roleIds.Contains(rpa.RoleId) && rpa.IsActive);

            var pageAccessIds = rolePageAccesses.Select(rpa => rpa.PageAccessId).Distinct();
            var pageAccesses = await _pageAccessRepository.FindAsync(pa =>
                pageAccessIds.Contains(pa.Id) && pa.IsActive);

            var result = new List<PageAccessDto>();
            foreach (var pageAccess in pageAccesses)
            {
                var rolePageAccess = rolePageAccesses.FirstOrDefault(rpa => rpa.PageAccessId == pageAccess.Id);
                var dto = _mapper.Map<PageAccessDto>(pageAccess);
                if (rolePageAccess != null)
                {
                    dto.CanView = rolePageAccess.CanView;
                    dto.CanCreate = rolePageAccess.CanCreate;
                    dto.CanEdit = rolePageAccess.CanEdit;
                    dto.CanDelete = rolePageAccess.CanDelete;
                    dto.CanExport = rolePageAccess.CanExport;
                }
                result.Add(dto);
            }

            return result.OrderBy(pa => pa.SortOrder);
        }

        public async Task<bool> HasPageAccessAsync(int userId, string pageUrl, string permission = "View")
        {
            var userRoles = await _userRoleRepository.FindAsync(ur =>
                ur.UserId == userId &&
                ur.IsActive &&
                (!ur.ExpiresAt.HasValue || ur.ExpiresAt > DateTime.UtcNow));

            var roleIds = userRoles.Select(ur => ur.RoleId);
            var pageAccess = await _pageAccessRepository.FirstOrDefaultAsync(pa => pa.PageUrl == pageUrl && pa.IsActive);

            if (pageAccess == null) return false;

            var rolePageAccess = await _rolePageAccessRepository.FirstOrDefaultAsync(rpa =>
                roleIds.Contains(rpa.RoleId) && rpa.PageAccessId == pageAccess.Id && rpa.IsActive);

            if (rolePageAccess == null) return false;

            return permission.ToLower() switch
            {
                "view" => rolePageAccess.CanView,
                "create" => rolePageAccess.CanCreate,
                "edit" => rolePageAccess.CanEdit,
                "delete" => rolePageAccess.CanDelete,
                "export" => rolePageAccess.CanExport,
                _ => false
            };
        }

        public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeUserId = null)
        {
            var query = await _userRepository.FindAsync(u => u.Username == username);
            if (excludeUserId.HasValue)
            {
                query = query.Where(u => u.Id != excludeUserId.Value);
            }
            return !query.Any();
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
