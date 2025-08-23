using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using AvyyanBackend.Interfaces;
using AvyyanBackend.DTOs;

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
                var roleDto = new RoleDto { RoleName = createRoleDto.Name, Description = createRoleDto.Description };
                var role = await _roleService.CreateRoleAsync(roleDto);
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
                var roleDto = new RoleDto { Id = id, RoleName = updateRoleDto.Name, Description = updateRoleDto.Description };
                var role = await _roleService.UpdateRoleAsync(roleDto);
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
                var pageAccess = await _roleService.CreatePageAccessAsync(createPageAccessDto);

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
                if (id != updatePageAccessDto.Id)
                    return BadRequest("ID mismatch");

                var pageAccess = await _roleService.UpdatePageAccessAsync(updatePageAccessDto);

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
        /// Delete a page access
        /// </summary>
        [HttpDelete("page-access/{id}")]
        public async Task<ActionResult> DeletePageAccess(int id)
        {
            try
            {
                var result = await _roleService.DeletePageAccessAsync(id);
                if (!result)
                {
                    return NotFound($"Page access with ID {id} not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting page access {PageAccessId}", id);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get all page accesses for a specific role
        /// </summary>
        [HttpGet("{roleId}/page-accesses")]
        public async Task<ActionResult<IEnumerable<PageAccessDto>>> GetRolePageAccesses(int roleId)
        {
            try
            {
                var pageAccesses = await _roleService.GetRolePageAccessesAsync(roleId);
                return Ok(pageAccesses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting page accesses for role {RoleId}", roleId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}