using System;
using System.Collections.Generic;
using System.Linq;
using Democrachat.Db;
using Democrachat.Db.Models;

namespace DemocrachatTest.Fakes
{
    public class FakeUserService : IUserService
    {
        public List<UserData> UserData = new();
        
        public UserData? GetDataById(int id)
        {
            return UserData.FirstOrDefault(u => u.Id == id);
        }

        public UserData? GetDataByUsername(string username)
        {
            return UserData.FirstOrDefault(u => u.Username == username);
        }

        public void AddMuteTime(int userId, TimeSpan time)
        {
            foreach (var userData in UserData.Where(userData => userData.Id == userId))
            {
                userData.MutedUntil = userData.MutedUntil.Add(time);
            }
        }

        public void SubtractSilver(int userId, int amount)
        {
            foreach (var userData in UserData.Where(userData => userData.Id == userId))
            {
                userData.Silver -= amount;
            }
        }

        public void AddSilver(int userId, int amount)
        {            
            foreach (var userData in UserData.Where(userData => userData.Id == userId))
            {
                userData.Silver += amount;
            }
        }

        public void AddGold(int userId, int amount)
        {
            foreach (var userData in UserData.Where(userData => userData.Id == userId))
            {
                userData.Gold += amount;
            }
        }

        public void UpdateKudoTime(int userId, DateTime time)
        {          
            foreach (var userData in UserData.Where(userData => userData.Id == userId))
            {
                userData.LastKudoTime = time;
            }
        }

        public void FinalizeNewUser(int id, string username, string password)
        {
            for (var i = 0; i < UserData.Count; i++)
            {
                var userData = UserData[i];
                if (userData.Id != id) continue;
                UserData[i] = userData with
                {
                    Username = username, 
                    Hash = BCrypt.Net.BCrypt.HashPassword(password)
                };
            }
        }

        public IEnumerable<UserData> GetOrderedUsersByWealth()
        {
            return UserData.OrderBy(u => u.Gold).ThenBy(u => u.Silver);
        }

        public IEnumerable<string> BatchGetUsernamesByIds(IEnumerable<int> ids)
        {
            return ids.Select(id => UserData.First(u => u.Id == id).Username);
        }

        public int RegisterUser()
        {
            var id = UserData.Count;
            UserData.Add(new UserData { Id = id, Username = "user23424"});
            return id;
        }
    }
}