using System;
using System.Text.Json.Serialization;
using Castle.Core.Internal;

namespace Democrachat.Db.Models
{
    public record UserData
    {
        public string Username { get; init;  }
        public bool IsGuest => Hash.IsNullOrEmpty();
        
        [JsonIgnore]
        public int Id { get; init;  }
        
        [JsonIgnore]
        public string Hash { get; init; }
        
        public DateTime MutedUntil { get; set; }

        public int Gold { get; set; }
        public int Silver { get; set; }
        public DateTime LastKudoTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}