using System.Security.Claims;
using Democrachat.Db;
using Microsoft.AspNetCore.SignalR;

namespace Democrachat.Chat
{
    public class UserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User.FindFirstValue("Id");
        }
    }
}