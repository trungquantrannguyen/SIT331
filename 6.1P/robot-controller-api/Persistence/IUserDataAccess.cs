using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public interface IUserDataAccess
{
    List<User> GetAllUsers();

    User? GetUserById(int id);

    User? GetUserByEmail(string email);

    List<User> GetAdminUsers();

    User AddUser(User user);

    bool UpdateUser(int id, User updatedUser);

    bool UpdateLoginDetails(int id, string email, string passwordHash);

    bool DeleteUser(int id);
}