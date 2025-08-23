using AutoMapper;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;

namespace AvyyanBackend.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<RoleMaster> _roleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<PageAccess> _pageAccessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;

        public RoleService(
            IUnitOfWork unitOfWork,
            IRepository<RoleMaster> roleRepository,
            IRepository<User> userRepository,
            IRepository<PageAccess> pageAccessRepository,
            IMapper mapper,
            ILogger<RoleService> logger)
        {
            _unitOfWork = unitOfWork;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _pageAccessRepository = pageAccessRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<RoleDto>>(roles);
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            return role != null ? _mapper.Map<RoleDto>(role) : null;
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            if (!await IsRoleNameUniqueAsync(roleDto.RoleName))
                throw new InvalidOperationException("Role already exists");

            var role = _mapper.Map<RoleMaster>(roleDto);

            await _roleRepository.AddAsync(role);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<RoleDto?> UpdateRoleAsync(RoleDto roleDto)
        {
            var role = await _roleRepository.GetByIdAsync(roleDto.Id);
            if (role == null) return null;

            if (role.RoleName != roleDto.RoleName && !await IsRoleNameUniqueAsync(roleDto.RoleName, roleDto.Id))
                throw new InvalidOperationException("Role name already exists");

            _mapper.Map(roleDto, role);

            _roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RoleDto>(role);
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null) return false;

            _roleRepository.Remove(role);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PageAccessDto>> GetAllPageAccessesAsync()
        {
            var pageAccesses = await _pageAccessRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PageAccessDto>>(pageAccesses);
        }

        public async Task<PageAccessDto?> GetPageAccessByIdAsync(int pageAccessId)
        {
            var pageAccess = await _pageAccessRepository.GetByIdAsync(pageAccessId);
            return pageAccess != null ? _mapper.Map<PageAccessDto>(pageAccess) : null;
        }

        public async Task<PageAccessDto> CreatePageAccessAsync(CreatePageAccessDto createPageAccessDto)
        {
            var pageAccess = _mapper.Map<PageAccess>(createPageAccessDto);

            await _pageAccessRepository.AddAsync(pageAccess);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PageAccessDto>(pageAccess);
        }

        public async Task<PageAccessDto?> UpdatePageAccessAsync(UpdatePageAccessDto updatePageAccessDto)
        {
            var pageAccess = await _pageAccessRepository.GetByIdAsync(updatePageAccessDto.Id);
            if (pageAccess == null) return null;

            _mapper.Map(updatePageAccessDto, pageAccess);

            _pageAccessRepository.Update(pageAccess);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PageAccessDto>(pageAccess);
        }

        public async Task<bool> DeletePageAccessAsync(int pageAccessId)
        {
            var pageAccess = await _pageAccessRepository.GetByIdAsync(pageAccessId);
            if (pageAccess == null) return false;

            _pageAccessRepository.Remove(pageAccess);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<PageAccessDto>> GetRolePageAccessesAsync(int roleId)
        {
            var pageAccesses = await _pageAccessRepository.FindAsync(pa => pa.RoleId == roleId);
            return _mapper.Map<IEnumerable<PageAccessDto>>(pageAccesses);
        }

        public async Task<bool> IsRoleNameUniqueAsync(string name, int? excludeRoleId = null)
        {
            var query = await _roleRepository.FindAsync(r => r.RoleName == name);
            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }
            return !query.Any();
        }

        public async Task<bool> IsPageNameUniqueAsync(string pageName, int? excludePageAccessId = null)
        {
            var query = await _pageAccessRepository.FindAsync(p => p.PageName == pageName);
            if (excludePageAccessId.HasValue)
            {
                query = query.Where(p => p.Id != excludePageAccessId.Value);
            }
            return !query.Any();
        }
    }
}