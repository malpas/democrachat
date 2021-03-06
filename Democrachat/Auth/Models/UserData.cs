using System.Text.Json.Serialization;
using Castle.Core.Internal;

namespace Democrachat.Auth.Models
{
    public record UserData
    {
        public string Username { get; init;  }
        public bool IsGuest => Hash.IsNullOrEmpty();
        
        [JsonIgnore]
        public int Id { get; init;  }
        
        [JsonIgnore]
        public string Hash { get; init; }
    }
}