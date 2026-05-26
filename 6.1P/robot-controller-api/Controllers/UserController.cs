using Microsoft.AspNetCore.Authorization;
using robot_controller_api.Services;
using Microsoft.AspNetCore.Mvc;
using robot_controller_api.Models;
using robot_controller_api.Persistence;

namespace robot_controller_api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserDataAccess _userDataAccess;

    public UsersController(IUserDataAccess userDataAccess, IPasswordHashService passwordHashService)
    {
        _userDataAccess = userDataAccess;
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAllUsers()
    {
        var users = _userDataAccess.GetAllUsers();

        foreach (var user in users)
        {
            user.PasswordHash = string.Empty;
        }

        return Ok(users);
    }

    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetAdminUsers()
    {
        var users = _userDataAccess.GetAdminUsers();

        foreach (var user in users)
        {
            user.PasswordHash = string.Empty;
        }

        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult GetUserById(int id)
    {
        var user = _userDataAccess.GetUserById(id);

        if (user == null)
        {
            return NotFound($"User with ID {id} was not found.");
        }

        user.PasswordHash = string.Empty;

        return Ok(user);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult RegisterUser([FromBody] User newUser)
    {
        if (newUser == null)
        {
            return BadRequest("User data is required.");
        }

        if (string.IsNullOrWhiteSpace(newUser.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(newUser.PasswordHash))
        {
            return BadRequest("Password is required.");
        }

        if (_userDataAccess.GetUserByEmail(newUser.Email) != null)
        {
            return Conflict("A user with this email already exists.");
        }

        var createdUser = _userDataAccess.AddUser(newUser);

        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        var existingUser = _userDataAccess.GetUserById(id);

        if (existingUser == null)
        {
            return NotFound($"User with ID {id} was not found.");
        }

        var success = _userDataAccess.UpdateUser(id, updatedUser);

        if (!success)
        {
            return BadRequest("User could not be updated.");
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteUser(int id)
    {
        var existingUser = _userDataAccess.GetUserById(id);

        if (existingUser == null)
        {
            return NotFound($"User with ID {id} was not found.");
        }

        _userDataAccess.DeleteUser(id);

        return NoContent();
    }

    [HttpPatch("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateUserLogin(int id, [FromBody] Login loginModel)
    {
        var existingUser = _userDataAccess.GetUserById(id);

        if (existingUser == null)
        {
            return NotFound($"User with ID {id} was not found.");
        }

        if (string.IsNullOrWhiteSpace(loginModel.Email))
        {
            return BadRequest("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(loginModel.Password))
        {
            return BadRequest("Password is required.");
        }

        var success = _userDataAccess.UpdateLoginDetails(
            id,
            loginModel.Email,
            loginModel.Password
        );

        if (!success)
        {
            return BadRequest("Login details could not be updated.");
        }

        return NoContent();
    }
}