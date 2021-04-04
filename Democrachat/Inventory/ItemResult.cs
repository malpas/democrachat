using System.Text.Json.Serialization;

namespace Democrachat.Inventory
{
    public record ItemResult(ItemResultType Type, string? Message);

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ItemResultType
    {
        Success,
        Error
    }
}