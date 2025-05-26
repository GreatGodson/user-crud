using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyFirstApi.Data;
using MyFirstApi.Models;
using MyFirstApi.Utilities;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IConfiguration config, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _config = config;
        private readonly AppDbContext _context = context;

        // POST: api/auth
        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse<User>>> CreateUser(CreateUserDto dto)
        {
            string? validateMail = FormValidator.ValidateEmail(dto.Email.Trim());
            string? validatePassword = FormValidator.ValidateStrongPassword(dto.Password.Trim());
            string? validateEmpty = FormValidator.ValidateEmpty(value: dto.Name.Trim(), errorMessage: "Name cannot be empty");


            if (validateMail != null || validateEmpty != null || validatePassword != null)
            {
                string? message = validateMail ?? validateEmpty ?? validatePassword ?? "";
                return BadRequest(new ApiResponse<User>(
                                       success: false,
                                        message: message,
                                        data: null
)
);
            }
            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (emailExists)
            {
                return BadRequest(new ApiResponse<User>(
           success: false,
           message: "Email is already in use.",
           data: null
       ));
            }

            User user = new()
            {
                Name = dto.Name.Trim(),
                Role = dto.Role.Trim(),
                Email = dto.Email.Trim(),
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password.Trim())
            };

            _ = _context.Users.Add(user);
            _ = await _context.SaveChangesAsync();

            return Created("api/user", new ApiResponse<User>(
                   success: true,
                   message: "User created successfully.",
                   data: user
               ));
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthenticatedModel>>> Login([FromBody] LoginModel loginModel)
        {
            string? validateEmail = FormValidator.ValidateEmail(loginModel.Username.Trim());
            string? validatePassword = FormValidator.ValidateEmpty(loginModel.Password.Trim(), errorMessage: "Password cannot be empty");


            if (validateEmail != null || validatePassword != null)
            {
                return BadRequest(new ApiResponse<User>(
                                       success: false,
                                        message: validateEmail ?? validatePassword ?? "",
                                        data: null
                ));
            }

            User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginModel.Username.ToLower());

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized(new ApiResponse<AuthenticatedModel>(
                    success: false,
                    message: "Invalid credentials.",
                    data: null
                ));
            }

            Claim[] claims =
            [
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role)
    ];

            string? secretKey = _config["Jwt:Key"];
            string? issuer = _config["Jwt:Issuer"];
            string? audience = _config["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(secretKey) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                return StatusCode(500, new ApiResponse<AuthenticatedModel>(
                    success: false,
                    message: "JWT configuration is incomplete.",
                    data: null
                ));
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            AuthenticatedModel responseData = new()
            {
                User = user,
                AccessToken = tokenString
            };

            return Ok(new ApiResponse<AuthenticatedModel>(
                success: true,
                message: "Login successful.",
                data: responseData
            ));
        }
    }
}