using System;
using System.Collections.Generic;
using System.Net;
using Democrachat.Db.Models;

namespace Democrachat.Db
{
    public interface IUserService
    {
        UserData? GetDataById(int id);
        UserData? GetDataByUsername(string username);
        void AddMuteTime(int userId, TimeSpan time);
        void SubtractSilver(int userId, int amount);
        void AddSilver(int userId, int amount);
        void AddGold(int userId, int amount);
        void UpdateKudoTime(int userId, DateTime time);
        void FinalizeNewUser(int id, string username, string password);
        IEnumerable<UserData> GetOrderedUsersByWealth();
        IEnumerable<string> BatchGetUsernamesByIds(IEnumerable<int> ids);
        int RegisterUser(IPAddress address);
        void AddLogin(int id, IPAddress address);
    }
}