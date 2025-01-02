using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using HotelBookingSystem.API.Models;

namespace HotelBookingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName))
            {
                return BadRequest("Invalid user data.");
            }

            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return StatusCode(500, new { Status = "Error", Message = "User already exists!" });
            }

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = System.Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(500, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }

            if (!await _roleManager.RoleExistsAsync("Guest"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Guest"));
            }

            if (await _roleManager.RoleExistsAsync("Guest"))
            {
                await _userManager.AddToRoleAsync(user, "Guest");
            }

            return Ok(new { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
        {
            if (login == null || string.IsNullOrEmpty(login.UserName))
            {
                return BadRequest("Invalid login data.");
            }

            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user != null && await _userManager.CheckPasswordAsync(user, login.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.NameIdentifier, user.Id ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole ?? string.Empty));
                }

                var token = GetToken(authClaims);
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = token.ValidTo
                };
                Response.Cookies.Append("jwt", tokenString, cookieOptions);

                return Ok(new { Message = "Login successful" });
            }
            return Unauthorized();
        }

       [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] User model)
        {
            if (model == null || string.IsNullOrEmpty(model.UserName))
            {
                return BadRequest("Invalid user data.");
            }

            var userExists = await _userManager.FindByNameAsync(model.UserName);
            if (userExists != null)
            {
                return StatusCode(500, new { Status = "Error", Message = "User already exists!" });
            }

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = System.Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(500, new { Status = "Error", Message = "User creation failed! Please check user details and try again." });
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (await _roleManager.RoleExistsAsync("Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }

            return Ok(new { Status = "Success", Message = "Admin user created successfully!" });
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            };
            Response.Cookies.Append("jwt", "", cookieOptions);

            return Ok(new { Message = "Logout successful" });
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: System.DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }

        [Authorize]
        [HttpGet("debug-auth")]
        public IActionResult DebugAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID is empty");
            }
            return Ok(new { UserId = userId });
        }

        [Authorize]
        [HttpGet("check-auth")]
        public async Task<IActionResult> CheckAuth()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var isAdmin = User.IsInRole("Admin");

            return Ok(new
            {
                isAuthenticated = true,
                isAdmin = isAdmin,
                username = user.UserName
            });
        }
    }

    public class LoginRequestDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
