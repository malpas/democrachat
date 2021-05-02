using System.Collections.Generic;
using System.Timers;

namespace Democrachat.Chat
{
    public class ChatSpamService : IChatSpamService
    {
        private Dictionary<int, int> chatBudget = new();

        public ChatSpamService()
        {
            var timer = new Timer { Interval = 10000 };
            timer.Elapsed += (_, _) =>
            {
                foreach (var userId in chatBudget.Keys)
                {
                    chatBudget[userId] += 1;
                    if (chatBudget[userId] > 7)
                    {
                        chatBudget[userId] = 7;
                    }
                }
            };
            timer.Start();
        }

        public bool CanSend(int userId)
        {
            if (!chatBudget.ContainsKey(userId))
                chatBudget[userId] = 7;
            return chatBudget[userId] >= 1;
        }

        public void MarkSend(int userId)
        {
            if (!chatBudget.ContainsKey(userId))
                chatBudget[userId] = 7;
            chatBudget[userId] -= 1;
        }
    }
}