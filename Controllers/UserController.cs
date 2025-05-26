using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstApi.Data;
using MyFirstApi.Models;
using MyFirstApi.Utilities;

namespace MyFirstApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/user/currentUser
        [HttpGet]
        public async Task<ActionResult<ApiResponse<User>>> GetCurrentUser()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {

                return Unauthorized(
                    new ApiResponse<User>(
                             success: false,
                                 message: "User identity not found.",
                                        data: null
                    )
                );

            }

            User? user = await _context.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            return user is null
                ? NotFound(new ApiResponse<User>(
                    success: false,
                    message: "User not found in database.",
                    data: null
                ))
                : Ok(new ApiResponse<User>(
                    success: true,
                    message: "User successfully retrieved",
                    data: user
                ));
        }

        // GET: api/user
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<List<User>>>> GetAllUsers()
        {
            List<User> users = await _context.Users.ToListAsync();
            return Ok(new ApiResponse<List<User>>(
                success: true,
                message: "Retrieved all users successfully",
                data: users
            ));
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> GetUserById(Guid id)

        {
            string? valdiateId = FormValidator.ValidateEmpty(id.ToString(), errorMessage: "Id cannot be empty");

            if (valdiateId != null)
            {
                return BadRequest(new ApiResponse<User>(
                    data: null,
                    message: valdiateId,
                    success: false
                ));
            }
            User? user = await _context.Users.FindAsync(id);
            return user is null
                ? NotFound(new ApiResponse<User>(
                    success: false,
                    message: $"User with id {id} not found.",
                    data: null
                ))
                : Ok(new ApiResponse<User>(
                    success: true,
                    message: "User retrieved succesfully",
                    data: user
                ));
        }

        // PATCH: api/user/{id}
        [HttpPatch("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> PartialUpdateUser(Guid id, [FromBody] UpdateUserDto updates)
        {

            string? valdiateId = FormValidator.ValidateEmpty(id.ToString(), errorMessage: "Id cannot be empty");

            if (valdiateId != null)
            {
                return BadRequest(new ApiResponse<User>(
                    data: null,
                    message: valdiateId,
                    success: false
                ));
            }
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<User>(
                    success: false,
                    message: "No user with record found",
                    data: null
                ));
            }

            if (!string.IsNullOrWhiteSpace(updates.Name))
            {
                user.Name = updates.Name;
            }

            if (!string.IsNullOrWhiteSpace(updates.Email))
            {
                user.Email = updates.Email;
            }

            if (!string.IsNullOrWhiteSpace(updates.Role))
            {
                user.Role = updates.Role;
            }

            _ = await _context.SaveChangesAsync();
            return Ok(new ApiResponse<User>(
                success: true,
                message: "User updated successfully",
                data: user
            ));
        }

        // DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<User>>> DeleteUser(Guid id)
        {

            string? valdiateId = FormValidator.ValidateEmpty(id.ToString(), errorMessage: "Id cannot be empty");

            if (valdiateId != null)
            {
                return BadRequest(new ApiResponse<User>(
                    data: null,
                    message: valdiateId,
                    success: false
                ));
            }
            User? user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new ApiResponse<User>(
                    success: false,
                    message: "User not found",
                    data: null
                ));
            }

            _ = _context.Users.Remove(user);
            _ = await _context.SaveChangesAsync();
            return
                new ApiResponse<User>(
                    success: true,
                    message: "Deleted successfully",
                    data: null


                )
            ;
        }
    }
}