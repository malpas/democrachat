using System;
using Democrachat.Auth.Models;

namespace Democrachat.Db
{
    public interface IUserService
    {
        UserData? GetDataById(int id);
        UserData? GetDataByUsername(string username);
        void AddMuteTime(int userId, TimeSpan time);
        void SubtractSilver(int userId, int amount);
    }
}