using System.Data;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Npgsql;
using UserService.Data;
using UserService.Model;
using UserService.Repository;
using visa_direct.Interfaces;

namespace UserService.Controllers
{
    //[Route("user")]  // Remove "api/[controller]", just use "user"
    [Route("api/v1/user/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContextPlSql _DbContext;
        private readonly AppDbContextMsSql _DbContextMsSql;
        private readonly UserRepository _repo;
        private readonly ILogger<UserController> _logger;
        private readonly ITransactionService _transactionService;


        public UserController(AppDbContextPlSql DbContext, AppDbContextMsSql appDbContextMsSql, UserRepository repo, ILogger<UserController> logger, ITransactionService transactionService)
        {
            this._DbContext = DbContext;
            this._DbContextMsSql = appDbContextMsSql;
            _repo = repo;
            this._logger = logger;
            _transactionService = transactionService;
        }


        //[Authorize]
        [Authorize(Roles = "OPERATOR,ADMIN")]
        [HttpGet("GET_ALL_USER")]
        public async Task<IActionResult> All()
        => Ok(await _repo.GetAllAsync());


        [Authorize(Roles = "OPERATOR,ADMIN")]
        [HttpGet("GET_USER_BY_ID/{id:int}")]
        public async Task<IActionResult> ById(int id)
        {
            var user = await _repo.GetByIdAsync(id);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            _logger.LogInformation("--Getting user by ID: {UserId}", id);
            try
            {
                var user = await _repo.GetByIdAsync(id);
                return user is null
                    ? NotFound(new { message = "User not found" })
                    : Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--Error getting user by ID: {UserId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }

            
        }

        [Authorize]
        [HttpPost("ADD_USER")]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequestDto request)
        {
            _logger.LogInformation("--Adding new user: {LoginName}", request.LoginName);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get the logged-in user's ID from JWT "uid" claim
                var uidClaim = User.FindFirst("uid");
                if (uidClaim == null || !int.TryParse(uidClaim.Value, out int loggedInUserId))
                {
                    return Unauthorized(new { success = false, message = "Unable to identify logged-in user from token" });
                }

                _logger.LogInformation("--User {LoginName} being created by logged-in user: {LoggedInUserId}",
                    request.LoginName, loggedInUserId);

                var result = await _repo.AddUserAsync(request, loggedInUserId);

                if (result.Success)
                {
                    _logger.LogInformation("--User added successfully: {LoginName} by user: {CreatedBy} with {GroupCount} groups",
                        request.LoginName, loggedInUserId, request.Groups?.Count ?? 0);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("--Failed to add user: {LoginName}, Error: {Error}",
                        request.LoginName, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--Error adding user: {LoginName}", request.LoginName);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while adding user"
                });
            }
        }

        [Authorize]
        [HttpPut("UPDATE_USER")]
        public async Task<IActionResult> UpdateUserFromBody([FromBody] UpdateUserRequestDto request)
        {
            _logger.LogInformation("--Updating user: {UserId}", request.UserId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validation - Only UserId is required now
            if (request.UserId <= 0)
            {
                return BadRequest(new { success = false, message = "Valid User ID is required" });
            }

            try
            {
                // Get the logged-in user's ID from JWT "uid" claim
                var uidClaim = User.FindFirst("uid");
                if (uidClaim == null || !int.TryParse(uidClaim.Value, out int loggedInUserId))
                {
                    return Unauthorized(new { success = false, message = "Unable to identify logged-in user from token" });
                }

                _logger.LogInformation("--User {UserId} being updated by logged-in user: {LoggedInUserId}",
                    request.UserId, loggedInUserId);

                // Pass the logged-in user ID to the repository
                var result = await _repo.UpdateUserAsync(request, loggedInUserId);

                if (result.Success)
                {
                    _logger.LogInformation("--User updated successfully: {UserId} by user: {ModifiedBy}",
                        request.UserId, loggedInUserId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("--Failed to update user: {UserId}, Error: {Error}",
                        request.UserId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--Error updating user: {UserId}", request.UserId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while updating user"
                });
            }
        }


        // Alternative endpoint that takes userId from body only (if you prefer this approach)
        [Authorize]
        [HttpPut("CHANGE_PASSWORD")]
        public async Task<IActionResult> ChangePasswordFromBody([FromBody] ChangePasswordRequestDto request)
        {
            _logger.LogInformation("--Changing password for user from body: {UserId}", request.UserId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validation
            if (request.UserId <= 0)
            {
                return BadRequest(new { success = false, message = "Valid User ID is required" });
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { success = false, message = "New password is required" });
            }

            if (string.IsNullOrWhiteSpace(request.OldPassword))
            {
                return BadRequest(new { success = false, message = "Current password is required" });
            }

            if (request.Password == request.OldPassword)
            {
                return BadRequest(new { success = false, message = "New password must be different from current password" });
            }

            try
            {
                // Get the logged-in user's ID from JWT "uid" claim
                var uidClaim = User.FindFirst("uid");
                if (uidClaim == null || !int.TryParse(uidClaim.Value, out int loggedInUserId))
                {
                    return Unauthorized(new { success = false, message = "Unable to identify logged-in user from token" });
                }

                var result = await _repo.ChangePasswordAsync(request, loggedInUserId);

                if (result.Success)
                {
                    _logger.LogInformation("--Password changed successfully for user: {UserId} by user: {ModifiedBy}",
                        request.UserId, loggedInUserId);
                    return Ok(result);
                }
                else
                {
                    _logger.LogWarning("--Failed to change password for user: {UserId}, Error: {Error}",
                        request.UserId, result.Message);
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "--Error changing password for user: {UserId}", request.UserId);
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error while changing password"
                });
            }
        }
        [Authorize]
        [HttpPost("test")]
        public async Task<IActionResult> test(string inParam, string spName, string provider)
        {
            return Ok(_transactionService.Process(inParam, spName, provider));
        }

    }
}
