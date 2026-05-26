using Microsoft.EntityFrameworkCore;
using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public class UserRepository : IUserDataAccess
{
    private const int BCryptWorkFactor = 12;

    private readonly RobotContext _context;

    public UserRepository(RobotContext context)
    {
        _context = context;
    }

    public List<User> GetAllUsers()
    {
        return _context.Users
            .AsNoTracking()
            .OrderBy(user => user.Id)
            .Select(user => ToSafeModel(user))
            .ToList();
    }

    public List<User> GetAdminUsers()
    {
        return _context.Users
            .AsNoTracking()
            .Where(user => user.Role != null && user.Role.ToLower() == "admin")
            .OrderBy(user => user.Id)
            .Select(user => ToSafeModel(user))
            .ToList();
    }

    public User? GetUserById(int id)
    {
        var user = _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Id == id);

        return user == null ? null : ToSafeModel(user);
    }

    public User? GetUserByEmail(string email)
    {
        var normalisedEmail = email.Trim().ToLower();

        var user = _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email.ToLower() == normalisedEmail);

        return user == null ? null : ToSafeModel(user);
    }

    public User? AuthenticateUser(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        var normalisedEmail = email.Trim().ToLower();

        var user = _context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Email.ToLower() == normalisedEmail);

        if (user == null)
        {
            return null;
        }

        var passwordIsValid = VerifyPassword(password, user.PasswordHash);

        if (!passwordIsValid)
        {
            return null;
        }

        return ToSafeModel(user);
    }

    public User AddUser(User user)
    {
        var now = DateTime.UtcNow;

        var entity = new UserEF
        {
            Email = user.Email.Trim(),
            FirstName = user.FirstName,
            LastName = user.LastName,
            PasswordHash = HashPassword(user.PasswordHash),
            Description = user.Description,
            Role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role,
            CreatedDate = now,
            ModifiedDate = now
        };

        _context.Users.Add(entity);
        _context.SaveChanges();

        return ToSafeModel(entity);
    }

    public bool UpdateUser(int id, User updatedUser)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);

        if (existingUser == null)
        {
            return false;
        }

        // PUT updates profile details only.
        // It deliberately does not update email or password.
        existingUser.FirstName = updatedUser.FirstName;
        existingUser.LastName = updatedUser.LastName;
        existingUser.Description = updatedUser.Description;
        existingUser.Role = updatedUser.Role;
        existingUser.ModifiedDate = DateTime.UtcNow;

        _context.SaveChanges();

        return true;
    }

    public bool UpdateLoginDetails(int id, string email, string password)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == id);

        if (existingUser == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        existingUser.Email = email.Trim();
        existingUser.PasswordHash = HashPassword(password);
        existingUser.ModifiedDate = DateTime.UtcNow;

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

    private static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.");
        }

        return BCrypt.Net.BCrypt.HashPassword(password, BCryptWorkFactor);
    }

    private static bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private static User ToSafeModel(UserEF user)
    {
        return new User
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PasswordHash = string.Empty,
            Description = user.Description,
            Role = user.Role,
            CreatedDate = user.CreatedDate,
            ModifiedDate = user.ModifiedDate
        };
    }
}