using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Dto.auth;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> userManager, TokenService service) : ControllerBase
    {
        private readonly TokenService _service = service;
        private readonly UserManager<AppUser> _userManager = userManager;

        [HttpPost("signUp")]
        public async Task<ActionResult> signUp([FromBody] RegisterDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Field Values" });
            }

            var roleName = string.IsNullOrEmpty(payload.Role) ? "User" : payload.Role;
            if (!new[] { "User", "Admin", "TaskManager" }.Contains(roleName))
            {
                return BadRequest(new { message = "Invalid user role selected" });
            }

            var user = new AppUser()
            {
                Email = payload.Email,
                FirstName = payload.FirstName,
                LastName = payload.LastName,
                UserName = payload.Email
            };

            if (await _userManager.FindByEmailAsync(payload.Email) is not null)
            {
                return BadRequest(new { message = "Email already exists!" });
            }

            var results = await _userManager.CreateAsync(user, payload.Password);

            if (!results.Succeeded) return BadRequest(new { message = "Failed to create account" });

            if (new[] { "User", "Admin", "TaskManager" }.Contains(roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }

            var roles = await _userManager.GetRolesAsync(user!);
            var token = _service.GenerateToken(user, roles);

            if (!results.Succeeded)
            {
                return BadRequest(new { message = "Failed to create account", error = results.Errors.Select(e => e.Description) });
            }

            return Ok(new
            {
                Token = token,
                User = new
                {
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Id
                },
                message = "Account Created successfully"
            });
        }

        [HttpPost("signIn")]
        public async Task<ActionResult> signIn([FromBody] LoginDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid field values", error = ModelState });
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid Email or Password. Please try again." });
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user!, payload.Password);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid Email or Password. Please try again." });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _service.GenerateToken(user, roles);

            return Ok(new
            {
                Token = token,
                message = "Successfully signed in.",
                User = new
                {
                    user.Id,
                    user.Email,
                    user.FirstName,
                    user.LastName
                }
            });
        }

        [HttpPost("resetPassword")]
        public async Task<ActionResult> resetPassword([FromBody] ResetDto payload)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "invalid field values", error = ModelState });
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Sorry, Email account is not valid!" });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Ok(new { message = "Token successfull sent" });
        }
    }
}
