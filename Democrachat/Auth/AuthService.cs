using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Democrachat.Auth.Models;
using Democrachat.Db;
using Democrachat.Db.Models;

namespace Democrachat.Auth
{
    public class AuthService : IAuthService
    {
        public static List<string> NaughtyWordList = new() {"cunt", "penis", "nigger", "fag", "pussy"};
        private IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public UserData? AttemptLogin(string username, string password, IPAddress? ip)
        {
            var userData = _userService.GetDataByUsername(username);
            if (userData == null)
            {
                return null;
            }
            var hash = userData?.Hash;
            if (hash != null && !BCrypt.Net.BCrypt.Verify(password, hash))
            {
                return null;
            }

            _userService.AddLogin(userData.Id, ip);
            return userData;
        }

        public RegistrationResult RegisterUser()
        {
            return new(_userService.RegisterUser());
        }

        public UserData? GetUserById(int id)
        {
            return _userService.GetDataById(id);
        }

        /// <summary>
        /// Convert a guest account into a named account
        /// </summary>
        /// <param name="id">User ID of unfinalized user</param>
        /// <param name="username">Username of new user</param>
        /// <param name="password">Password to use</param>
        /// <exception cref="InvalidOperationException">If finalization is invalid</exception>
        public void FinalizeNewUser(int id, string username, string password)
        {           
            var userData = _userService.GetDataById(id);
            if (userData == null)
                throw new InvalidOperationException();
            if (!userData.IsGuest)
                throw new InvalidOperationException($"Cannot rename full account");
            if (IsUsernameTaken(username) || NaughtyWordList.Any(word => username.Contains(word)))
                throw new InvalidOperationException($"User \"{username}\" is already taken");
            _userService.FinalizeNewUser(id, username, password);
        }

        public bool IsUsernameTaken(string username)
        {
            return _userService.GetDataByUsername(username) != null;
        }

        public IEnumerable<string> BatchGetUsernamesByIds(IEnumerable<int> ids)
        {
            return _userService.BatchGetUsernamesByIds(ids);
        }

        public IEnumerable<UserData> GetOrderedUsersWithWealth()
        {
            return _userService.GetOrderedUsersByWealth();
        }
    }
}