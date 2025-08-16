using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;

        public RoleController(IRoleService roleService, ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            try
            {
                var roles = await _roleService.GetAllRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all roles");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get role by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            try
            {
                var role = await _roleService.GetRoleByIdAsync(id);
                if (role == null)
                    return NotFound("Role not found");

                return Ok(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create new role
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(createRoleDto.Name, createRoleDto.Description);
                return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update role
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            try
            {
                var role = await _roleService.UpdateRoleAsync(id, updateRoleDto.Name, updateRoleDto.Description);
                if (role == null)
                    return NotFound("Role not found");

                return Ok(role);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete role
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole(int id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                if (!result)
                    return NotFound("Role not found or cannot delete system role");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get all page accesses
        /// </summary>
        [HttpGet("page-accesses")]
        public async Task<ActionResult<IEnumerable<PageAccessDto>>> GetAllPageAccesses()
        {
            try
            {
                var pageAccesses = await _roleService.GetAllPageAccessesAsync();
                return Ok(pageAccesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting page accesses");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get page access by ID
        /// </summary>
        [HttpGet("page-accesses/{id}")]
        public async Task<ActionResult<PageAccessDto>> GetPageAccess(int id)
        {
            try
            {
                var pageAccess = await _roleService.GetPageAccessByIdAsync(id);
                if (pageAccess == null)
                    return NotFound("Page access not found");

                return Ok(pageAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting page access");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create new page access
        /// </summary>
        [HttpPost("page-accesses")]
        public async Task<ActionResult<PageAccessDto>> CreatePageAccess([FromBody] CreatePageAccessDto createPageAccessDto)
        {
            try
            {
                var pageAccess = await _roleService.CreatePageAccessAsync(
                    createPageAccessDto.PageName,
                    createPageAccessDto.PageUrl,
                    createPageAccessDto.Description,
                    createPageAccessDto.Category,
                    createPageAccessDto.Icon,
                    createPageAccessDto.SortOrder,
                    createPageAccessDto.IsMenuItem);

                return CreatedAtAction(nameof(GetPageAccess), new { id = pageAccess.Id }, pageAccess);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating page access");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update page access
        /// </summary>
        [HttpPut("page-accesses/{id}")]
        public async Task<ActionResult<PageAccessDto>> UpdatePageAccess(int id, [FromBody] UpdatePageAccessDto updatePageAccessDto)
        {
            try
            {
                var pageAccess = await _roleService.UpdatePageAccessAsync(
                    id,
                    updatePageAccessDto.PageName,
                    updatePageAccessDto.PageUrl,
                    updatePageAccessDto.Description,
                    updatePageAccessDto.Category,
                    updatePageAccessDto.Icon,
                    updatePageAccessDto.SortOrder,
                    updatePageAccessDto.IsMenuItem);

                if (pageAccess == null)
                    return NotFound("Page access not found");

                return Ok(pageAccess);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating page access");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete page access
        /// </summary>
        [HttpDelete("page-accesses/{id}")]
        public async Task<ActionResult> DeletePageAccess(int id)
        {
            try
            {
                var result = await _roleService.DeletePageAccessAsync(id);
                if (!result)
                    return NotFound("Page access not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting page access");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get role page accesses
        /// </summary>
        [HttpGet("{id}/page-accesses")]
        public async Task<ActionResult<IEnumerable<PageAccessDto>>> GetRolePageAccesses(int id)
        {
            try
            {
                var pageAccesses = await _roleService.GetRolePageAccessesAsync(id);
                return Ok(pageAccesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting role page accesses");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Grant page access to role
        /// </summary>
        [HttpPost("{id}/page-accesses/{pageAccessId}")]
        public async Task<ActionResult> GrantPageAccess(int id, int pageAccessId, [FromBody] GrantPageAccessDto grantPageAccessDto)
        {
            try
            {
                var result = await _roleService.GrantPageAccessToRoleAsync(
                    id,
                    pageAccessId,
                    grantPageAccessDto.CanView,
                    grantPageAccessDto.CanCreate,
                    grantPageAccessDto.CanEdit,
                    grantPageAccessDto.CanDelete,
                    grantPageAccessDto.CanExport);

                if (!result)
                    return BadRequest("Failed to grant page access");

                return Ok(new { message = "Page access granted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while granting page access");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Revoke page access from role
        /// </summary>
        [HttpDelete("{id}/page-accesses/{pageAccessId}")]
        public async Task<ActionResult> RevokePageAccess(int id, int pageAccessId)
        {
            try
            {
                var result = await _roleService.RevokePageAccessFromRoleAsync(id, pageAccessId);
                if (!result)
                    return NotFound("Page access assignment not found");

                return Ok(new { message = "Page access revoked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while revoking page access");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get users in role
        /// </summary>
        [HttpGet("{id}/users")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersInRole(int id)
        {
            try
            {
                var users = await _roleService.GetUsersInRoleAsync(id);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting users in role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Assign role to user
        /// </summary>
        [HttpPost("{id}/users")]
        public async Task<ActionResult> AssignRoleToUser(int id, [FromBody] AssignRoleToUserDto assignRoleToUserDto)
        {
            try
            {
                var assignRoleDto = new AssignRoleDto
                {
                    UserId = assignRoleToUserDto.UserId,
                    RoleId = id,
                    ExpiresAt = assignRoleToUserDto.ExpiresAt
                };

                var result = await _roleService.AssignRoleToUserAsync(assignRoleDto);
                if (!result)
                    return BadRequest("Failed to assign role to user");

                return Ok(new { message = "Role assigned to user successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning role to user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        [HttpDelete("{id}/users/{userId}")]
        public async Task<ActionResult> RemoveRoleFromUser(int id, int userId)
        {
            try
            {
                var result = await _roleService.RemoveRoleFromUserAsync(userId, id);
                if (!result)
                    return NotFound("Role assignment not found");

                return Ok(new { message = "Role removed from user successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing role from user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
