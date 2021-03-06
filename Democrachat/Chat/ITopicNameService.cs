using System.Collections;
using System.Collections.Generic;

namespace Democrachat.Chat
{
    public interface ITopicNameService
    {
        bool IsValidTopic(string name);
        IEnumerable<string> GetTopics();
    }
}