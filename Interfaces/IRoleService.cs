using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IRoleService
    {
        // Role Management
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int roleId);
        Task<RoleDto> CreateRoleAsync(RoleDto roleDto);
        Task<RoleDto?> UpdateRoleAsync(RoleDto roleDto);
        Task<bool> DeleteRoleAsync(int roleId);

        // Page Access Management
        Task<IEnumerable<PageAccessDto>> GetAllPageAccessesAsync();
        Task<PageAccessDto?> GetPageAccessByIdAsync(int pageAccessId);
        Task<PageAccessDto> CreatePageAccessAsync(CreatePageAccessDto createPageAccessDto);
        Task<PageAccessDto?> UpdatePageAccessAsync(UpdatePageAccessDto updatePageAccessDto);
        Task<bool> DeletePageAccessAsync(int pageAccessId);

        // Role Page Access Management
        Task<IEnumerable<PageAccessDto>> GetRolePageAccessesAsync(int roleId);

        // Validation
        Task<bool> IsRoleNameUniqueAsync(string name, int? excludeRoleId = null);
        Task<bool> IsPageNameUniqueAsync(string pageName, int? excludePageAccessId = null);
    }
}