using Democrachat.Auth.Models;

namespace Democrachat.Auth
{
    public interface IAuthService
    {
        UserData? AttemptLogin(string username, string password);
        RegistrationResult RegisterUser();
        UserData GetUserById(int id);
        void FinalizeNewUser(int id, string username, string password);
        bool IsUsernameTaken(string username);
    }
}