using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AvyyanBackend.DTOs;
using AvyyanBackend.Interfaces;
using System.Security.Claims;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get current user profile
        /// </summary>
        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update current user profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<ActionResult<UserDto>> UpdateProfile(UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var user = await _userService.UpdateProfileAsync(userId.Value, updateUserDto);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Change current user password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var result = await _userService.ChangePasswordAsync(userId.Value, changePasswordDto);
                if (!result)
                    return BadRequest("Current password is incorrect");

                return Ok(new { message = "Password changed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing password");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get user's page access permissions
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<PageAccessDto>>> GetPermissions()
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var permissions = await _userService.GetUserPageAccessesAsync(userId.Value);
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user permissions");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Check if current user has specific page access
        /// </summary>
        [HttpGet("permissions/check")]
        public async Task<ActionResult<bool>> CheckPermission([FromQuery] string pageUrl, [FromQuery] string permission = "View")
        {
            try
            {
                var userId = GetUserId();
                if (!userId.HasValue)
                    return Unauthorized();

                var hasAccess = await _userService.HasPageAccessAsync(userId.Value, pageUrl, permission);
                return Ok(hasAccess);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking permission");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all users");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Create new user (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Update user (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, updateUserDto);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(id);
                if (!result)
                    return NotFound("User not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Lock user account (Admin only)
        /// </summary>
        [HttpPost("{id}/lock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> LockUser(int id)
        {
            try
            {
                var result = await _userService.LockUserAsync(id);
                if (!result)
                    return NotFound("User not found");

                return Ok(new { message = "User locked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while locking user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Unlock user account (Admin only)
        /// </summary>
        [HttpPost("{id}/unlock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UnlockUser(int id)
        {
            try
            {
                var result = await _userService.UnlockUserAsync(id);
                if (!result)
                    return NotFound("User not found");

                return Ok(new { message = "User unlocked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unlocking user");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Assign role to user (Admin only)
        /// </summary>
        [HttpPost("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRole(int id, AssignRoleDto assignRoleDto)
        {
            try
            {
                assignRoleDto.UserId = id; // Ensure the user ID matches the route
                var result = await _userService.AssignRoleToUserAsync(assignRoleDto);
                if (!result)
                    return BadRequest("Failed to assign role");

                return Ok(new { message = "Role assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Remove role from user (Admin only)
        /// </summary>
        [HttpDelete("{id}/roles/{roleId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> RemoveRole(int id, int roleId)
        {
            try
            {
                var result = await _userService.RemoveRoleFromUserAsync(id, roleId);
                if (!result)
                    return NotFound("Role assignment not found");

                return Ok(new { message = "Role removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing role");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        /// <summary>
        /// Get user roles (Admin only)
        /// </summary>
        [HttpGet("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(int id)
        {
            try
            {
                var roles = await _userService.GetUserRolesAsync(id);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting user roles");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
