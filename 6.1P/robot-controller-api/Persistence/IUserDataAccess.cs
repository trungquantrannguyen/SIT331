using robot_controller_api.Models;

namespace robot_controller_api.Persistence;

public interface IUserDataAccess
{
    List<User> GetAllUsers();

    List<User> GetAdminUsers();

    User? GetUserById(int id);

    User? GetUserByEmail(string email);

    User? AuthenticateUser(string email, string password);

    User AddUser(User user);

    bool UpdateUser(int id, User updatedUser);

    bool UpdateLoginDetails(int id, string email, string password);

    bool DeleteUser(int id);
}