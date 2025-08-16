using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<PageAccess> _pageAccessRepository;
        private readonly IRepository<RolePageAccess> _rolePageAccessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IUnitOfWork unitOfWork,
            IRepository<Role> roleRepository,
            IRepository<User> userRepository,
            IRepository<UserRole> userRoleRepository,
            IRepository<PageAccess> pageAccessRepository,
            IRepository<RolePageAccess> rolePageAccessRepository,
            IMapper mapper,
            ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _pageAccessRepository = pageAccessRepository;
            _rolePageAccessRepository = rolePageAccessRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.FindAsync(r => r.IsActive);
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            return role != null && role.IsActive ? _mapper.Map<RoleDto>(role) : null;
        }

        public async Task<RoleDto> CreateRoleAsync(string name, string? description = null)
        {
            if (!await IsRoleNameUniqueAsync(name))
                throw new InvalidOperationException("Role already exists");

            var role = new Role
            {
                Name = name,
                Description = description,
                IsSystemRole = false
            };

            await _roleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto?> UpdateRoleAsync(int roleId, string name, string? description = null)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null || !role.IsActive) return null;

            if (role.IsSystemRole)
                throw new InvalidOperationException("Cannot update system roles");

            if (role.Name != name && !await IsRoleNameUniqueAsync(name, roleId))
                throw new InvalidOperationException("Role name already exists");

            role.Name = name;
            role.Description = description;
            role.UpdatedAt = DateTime.UtcNow;

            _roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null || role.IsSystemRole) return false;

            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;

            _roleRepository.Update(role);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PageAccessDto>> GetAllPageAccessesAsync()
        {
            var pageAccesses = await _pageAccessRepository.FindAsync(pa => pa.IsActive);
            return _mapper.Map<IEnumerable<PageAccessDto>>(pageAccesses);
        }

        public async Task<PageAccessDto?> GetPageAccessByIdAsync(int pageAccessId)
        {
            var pageAccess = await _pageAccessRepository.GetByIdAsync(pageAccessId);
            return pageAccess != null && pageAccess.IsActive ? _mapper.Map<PageAccessDto>(pageAccess) : null;
        }

        public async Task<PageAccessDto> CreatePageAccessAsync(string pageName, string pageUrl, string? description = null, string? category = null, string? icon = null, int sortOrder = 0, bool isMenuItem = true)
        {
            if (!await IsPageUrlUniqueAsync(pageUrl))
                throw new InvalidOperationException("Page URL already exists");

            var pageAccess = new PageAccess
            {
                PageName = pageName,
                PageUrl = pageUrl,
                Description = description,
                Category = category,
                Icon = icon,
                SortOrder = sortOrder,
                IsMenuItem = isMenuItem
            };

            await _pageAccessRepository.AddAsync(pageAccess);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PageAccessDto>(pageAccess);
        }

        public async Task<PageAccessDto?> UpdatePageAccessAsync(int pageAccessId, string pageName, string pageUrl, string? description = null, string? category = null, string? icon = null, int sortOrder = 0, bool isMenuItem = true)
        {
            var pageAccess = await _pageAccessRepository.GetByIdAsync(pageAccessId);
            if (pageAccess == null || !pageAccess.IsActive) return null;

            if (pageAccess.PageUrl != pageUrl && !await IsPageUrlUniqueAsync(pageUrl, pageAccessId))
                throw new InvalidOperationException("Page URL already exists");

            pageAccess.PageName = pageName;
            pageAccess.PageUrl = pageUrl;
            pageAccess.Description = description;
            pageAccess.Category = category;
            pageAccess.Icon = icon;
            pageAccess.SortOrder = sortOrder;
            pageAccess.IsMenuItem = isMenuItem;
            pageAccess.UpdatedAt = DateTime.UtcNow;

            _pageAccessRepository.Update(pageAccess);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PageAccessDto>(pageAccess);
        }

        public async Task<bool> DeletePageAccessAsync(int pageAccessId)
        {
            var pageAccess = await _pageAccessRepository.GetByIdAsync(pageAccessId);
            if (pageAccess == null) return false;

            pageAccess.IsActive = false;
            pageAccess.UpdatedAt = DateTime.UtcNow;

            _pageAccessRepository.Update(pageAccess);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PageAccessDto>> GetRolePageAccessesAsync(int roleId)
        {
            var rolePageAccesses = await _rolePageAccessRepository.FindAsync(rpa => 
                rpa.RoleId == roleId && rpa.IsActive);

            var pageAccessIds = rolePageAccesses.Select(rpa => rpa.PageAccessId);
            var pageAccesses = await _pageAccessRepository.FindAsync(pa => 
                pageAccessIds.Contains(pa.Id) && pa.IsActive);

            var result = new List<PageAccessDto>();
            foreach (var pageAccess in pageAccesses)
            {
                var rolePageAccess = rolePageAccesses.First(rpa => rpa.PageAccessId == pageAccess.Id);
                var dto = _mapper.Map<PageAccessDto>(pageAccess);
                dto.CanView = rolePageAccess.CanView;
                dto.CanCreate = rolePageAccess.CanCreate;
                dto.CanEdit = rolePageAccess.CanEdit;
                dto.CanDelete = rolePageAccess.CanDelete;
                dto.CanExport = rolePageAccess.CanExport;
                result.Add(dto);
            }

            return result.OrderBy(pa => pa.SortOrder);
        }

        public async Task<bool> GrantPageAccessToRoleAsync(int roleId, int pageAccessId, bool canView = true, bool canCreate = false, bool canEdit = false, bool canDelete = false, bool canExport = false)
        {
            var existingAccess = await _rolePageAccessRepository.FirstOrDefaultAsync(rpa => 
                rpa.RoleId == roleId && rpa.PageAccessId == pageAccessId);

            if (existingAccess != null)
            {
                existingAccess.CanView = canView;
                existingAccess.CanCreate = canCreate;
                existingAccess.CanEdit = canEdit;
                existingAccess.CanDelete = canDelete;
                existingAccess.CanExport = canExport;
                existingAccess.UpdatedAt = DateTime.UtcNow;

                _rolePageAccessRepository.Update(existingAccess);
            }
            else
            {
                var rolePageAccess = new RolePageAccess
                {
                    RoleId = roleId,
                    PageAccessId = pageAccessId,
                    CanView = canView,
                    CanCreate = canCreate,
                    CanEdit = canEdit,
                    CanDelete = canDelete,
                    CanExport = canExport,
                    GrantedAt = DateTime.UtcNow
                };

                await _rolePageAccessRepository.AddAsync(rolePageAccess);
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> RevokePageAccessFromRoleAsync(int roleId, int pageAccessId)
        {
            var rolePageAccess = await _rolePageAccessRepository.FirstOrDefaultAsync(rpa => 
                rpa.RoleId == roleId && rpa.PageAccessId == pageAccessId);

            if (rolePageAccess == null) return false;

            _rolePageAccessRepository.Remove(rolePageAccess);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRolePageAccessAsync(int roleId, int pageAccessId, bool canView = true, bool canCreate = false, bool canEdit = false, bool canDelete = false, bool canExport = false)
        {
            return await GrantPageAccessToRoleAsync(roleId, pageAccessId, canView, canCreate, canEdit, canDelete, canExport);
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

        public async Task<IEnumerable<UserDto>> GetUsersInRoleAsync(int roleId)
        {
            var userRoles = await _userRoleRepository.FindAsync(ur => 
                ur.RoleId == roleId && 
                ur.IsActive && 
                (!ur.ExpiresAt.HasValue || ur.ExpiresAt > DateTime.UtcNow));

            var userIds = userRoles.Select(ur => ur.UserId);
            var users = await _userRepository.FindAsync(u => userIds.Contains(u.Id) && u.IsActive);

            return _mapper.Map<IEnumerable<UserDto>>(users);
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

        public async Task<bool> IsRoleNameUniqueAsync(string name, int? excludeRoleId = null)
        {
            var query = await _roleRepository.FindAsync(r => r.Name == name);
            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }
            return !query.Any();
        }

        public async Task<bool> IsPageUrlUniqueAsync(string pageUrl, int? excludePageAccessId = null)
        {
            var query = await _pageAccessRepository.FindAsync(pa => pa.PageUrl == pageUrl);
            if (excludePageAccessId.HasValue)
            {
                query = query.Where(pa => pa.Id != excludePageAccessId.Value);
            }
            return !query.Any();
        }
    }
}
