using Microsoft.EntityFrameworkCore;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class UserRepository : IUserDataAccess
{
    private readonly RobotContext _context;

    public UserRepository(RobotContext context)
    {
        _context = context;
    }

    public List<User> GetAllUsers()
    {
        return _context.Users
            .Select(user => new User
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Description = user.Description,
                Role = user.Role,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate
            })
            .ToList();
    }

    public List<User> GetAdminUsers()
    {
        return _context.Users
            .Where(user => user.Role != null && user.Role.ToLower() == "admin")
            .Select(user => new User
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Description = user.Description,
                Role = user.Role,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate
            })
            .ToList();
    }

    public User? GetUserById(int id)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return null;
        }
        return new User
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Description = user.Description,
            Role = user.Role,
            CreatedDate = user.CreatedDate,
            ModifiedDate = user.ModifiedDate
        };
    }

    public User? GetUserByEmail(string email)
    {
        var user = _context.Users
            .FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

        if (user == null)
        {
            return null;
        }

        return new User
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Description = user.Description,
            Role = user.Role,
            CreatedDate = user.CreatedDate,
            ModifiedDate = user.ModifiedDate
        };
    }

    public User AddUser(User user)
    {
        var entity = new UserEF
        {
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PasswordHash = user.PasswordHash,
            Description = user.Description,
            Role = user.Role,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        _context.Users.Add(entity);
        _context.SaveChanges();

        user.Id = entity.Id;
        user.CreatedDate = entity.CreatedDate;
        user.ModifiedDate = entity.ModifiedDate;

        return user;
    }

    public bool UpdateUser(int id, User updatedUser)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);

        if (existingUser == null)
        {
            return false;
        }

        // The task says PUT should disregard password and email change.
        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Description = updatedUser.Description;
        existingUser.Role = updatedUser.Role;
        existingUser.ModifiedDate = DateTime.Now;

        _context.SaveChanges();

        return true;
    }

    public bool UpdateLoginDetails(int id, string email, string passwordHash)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);

        if (existingUser == null)
        {
            return false;
        }

        existingUser.Email = email;
        existingUser.PasswordHash = passwordHash;
        existingUser.ModifiedDate = DateTime.Now;

        _context.SaveChanges();

        return true;
    }

    public bool DeleteUser(int id)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);

        if (existingUser == null)
        {
            return false;
        }

        _context.Users.Remove(existingUser);
        _context.SaveChanges();

        return true;
    }
}