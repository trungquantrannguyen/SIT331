namespace robot_controller_api.Services;

public interface IPasswordHashService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}