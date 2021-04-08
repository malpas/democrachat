using System;
using System.Net;
using System.Security.Cryptography;
using Democrachat.Chat;
using Democrachat.Db;
using Democrachat.Db.Models;
using Democrachat.Log;
using Microsoft.AspNetCore.SignalR;
using Priority_Queue;

namespace Democrachat.Kudo
{
    public class KudoService : IKudoService
    {
        private IKudoTableService _kudoTableService;
        private IItemService _itemService;
        private IUserService _userService;
        private ILogger _logger;
        private IHubContext<ChatHub> _chatHubContext;

        public KudoService(IKudoTableService kudoTableService, IItemService itemService, IUserService userService,
            ILogger logger, IHubContext<ChatHub> chatHubContext)
        {
            _kudoTableService = kudoTableService;
            _itemService = itemService;
            _userService = userService;
            _logger = logger;
            _chatHubContext = chatHubContext;
        }

        /// <summary>
        /// Give an item to a user based on the weighted listings in the "kudo table",
        /// which specifies possible item templates the recipient can get
        /// </summary>
        /// <param name="fromUserId">ID of user giving kudo</param>
        /// <param name="toUsername">ID of user receiving kudo</param>
        /// <param name="fromIp"></param>
        /// <exception cref="InvalidOperationException">If no items in kudo table or recipient does not exist</exception>
        public void GiveKudo(int fromUserId, string toUsername, IPAddress? fromIp)
        {
            var fromUser = _userService.GetDataById(fromUserId);
            if (toUsername == fromUser.Username)
            {
                throw new InvalidOperationException("Cannot send kudo to yourself");
            }
            if (DateTime.Now < fromUser.LastKudoTime.AddHours(8))
            {
                throw new InvalidOperationException("Can only send kudo every 8 hours");
            }
            if (DateTime.Now < fromUser.CreatedAt.AddDays(3))
            {
                throw new InvalidOperationException("Account too recent");
            }
            var templateId = SelectItem();
            var toUser = _userService.GetDataByUsername(toUsername);
            if (toUser == null)
            {
                throw new InvalidOperationException($"Could not get user \"{toUsername}\"");
            }
            _itemService.CreateItem(toUser.Id, templateId);
            _userService.UpdateKudoTime(fromUserId, DateTime.Now);

            if (fromIp == null) return;
            var hash = SHA256.HashData(fromIp.GetAddressBytes());
            var hashText = BitConverter.ToString(hash).Replace("-", "").Substring(3, 5).ToLower();
            _logger.WriteLog($"kudo from={fromUser.Username} to={toUsername} hash={hashText}");
            _chatHubContext.Clients.User(toUser.Id.ToString()).SendCoreAsync("ReceiveMessage", new object? []{"all", "cc", $"{fromUser.Username} just sent you a kudo. Check your inventory!"});
        }

        private int SelectItem()
        {
            var sum = 0;
            var queue = new SimplePriorityQueue<KudoListing, int>();
            foreach (var kudoListing in _kudoTableService.GetPossibleKudoItems())
            {
                queue.Enqueue(kudoListing, kudoListing.Weight);
                sum += kudoListing.Weight;
            } 
            if (queue.Count == 0)
            {
                throw new InvalidOperationException("No kudo items found");
            }

            var rand = new Random();
            var selection = rand.Next(sum);
            while (true)
            {
                var listing = queue.Dequeue();
                selection -= listing.Weight;
                
                if (selection >= 0) continue;
                return listing.TemplateId;
            }
        }
    }
}