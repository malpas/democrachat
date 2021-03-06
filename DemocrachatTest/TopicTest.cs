using System.Collections.Generic;
using Democrachat.Chat;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    public class TopicTest
    {
        [Fact]
        public void CanGetTopics()
        {
            var mockTopicService = new Mock<ITopicNameService>();
            var topics = new List<string> {"architecture", "general"};
            mockTopicService.Setup(s => s.GetTopics()).Returns(topics);
            var topicController = new TopicsController(mockTopicService.Object);
            Assert.Equal(topics, topicController.GetTopics());
        }
    }
}