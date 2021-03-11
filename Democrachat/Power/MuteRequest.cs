using System;
using System.ComponentModel.DataAnnotations;

namespace Democrachat.Chat
{
    public record MuteRequest
    {
        public string Username { get; init; }
        
        [Range(1, Int32.MaxValue, ErrorMessage = "Silver is required to mute")]
        public int Silver { get; set; }
    }
}