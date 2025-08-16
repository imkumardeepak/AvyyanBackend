using AvyyanBackend.DTOs;

namespace AvyyanBackend.Interfaces
{
    public interface IRoleService
    {
        // Role Management
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int roleId);
        Task<RoleDto> CreateRoleAsync(string name, string? description = null);
        Task<RoleDto?> UpdateRoleAsync(int roleId, string name, string? description = null);
        Task<bool> DeleteRoleAsync(int roleId);

        // Page Access Management
        Task<IEnumerable<PageAccessDto>> GetAllPageAccessesAsync();
        Task<PageAccessDto?> GetPageAccessByIdAsync(int pageAccessId);
        Task<PageAccessDto> CreatePageAccessAsync(string pageName, string pageUrl, string? description = null, string? category = null, string? icon = null, int sortOrder = 0, bool isMenuItem = true);
        Task<PageAccessDto?> UpdatePageAccessAsync(int pageAccessId, string pageName, string pageUrl, string? description = null, string? category = null, string? icon = null, int sortOrder = 0, bool isMenuItem = true);
        Task<bool> DeletePageAccessAsync(int pageAccessId);

        // Role Page Access Management
        Task<IEnumerable<PageAccessDto>> GetRolePageAccessesAsync(int roleId);
        Task<bool> GrantPageAccessToRoleAsync(int roleId, int pageAccessId, bool canView = true, bool canCreate = false, bool canEdit = false, bool canDelete = false, bool canExport = false);
        Task<bool> RevokePageAccessFromRoleAsync(int roleId, int pageAccessId);
        Task<bool> UpdateRolePageAccessAsync(int roleId, int pageAccessId, bool canView = true, bool canCreate = false, bool canEdit = false, bool canDelete = false, bool canExport = false);

        // Role Assignment
        Task<bool> AssignRoleToUserAsync(AssignRoleDto assignRoleDto);
        Task<bool> RemoveRoleFromUserAsync(int userId, int roleId);
        Task<IEnumerable<UserDto>> GetUsersInRoleAsync(int roleId);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);

        // Validation
        Task<bool> IsRoleNameUniqueAsync(string name, int? excludeRoleId = null);
        Task<bool> IsPageUrlUniqueAsync(string pageUrl, int? excludePageAccessId = null);
    }
}
