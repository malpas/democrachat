using System.Collections.Generic;
using Democrachat.Auth.Models;
using Democrachat.Db.Models;

namespace Democrachat.Auth
{
    public interface IAuthService
    {
        UserData? AttemptLogin(string username, string password);
        RegistrationResult RegisterUser();
        UserData GetUserById(int id);
        void FinalizeNewUser(int id, string username, string password);
        bool IsUsernameTaken(string username);
        IEnumerable<string> BatchGetUsernamesByIds(IEnumerable<int> id);
    }
}