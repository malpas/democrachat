using System.Text.Json.Serialization;

namespace Democrachat.Auth.Models
{
    public record UserData
    {
        public string Username { get; init;  }
        
        [JsonIgnore]
        public int Id { get; init;  }
    }
}