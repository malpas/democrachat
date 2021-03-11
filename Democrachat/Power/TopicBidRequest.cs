using System;
using System.ComponentModel.DataAnnotations;

namespace Democrachat.Power
{
    public record TopicBidRequest
    {
        [MinLength(5, ErrorMessage = "Name must be >=5 chars")]
        [MaxLength(15, ErrorMessage = "Name must be <=15 chars")]
        [RegularExpression("[A-Za-z]+", ErrorMessage = "Name must contain only letters")]
        public string Name { get; init; }
        
        [Range(5, Int32.MaxValue, ErrorMessage = "Must bid at least 5 silver")]
        public int Silver { get; init; }
    }
}