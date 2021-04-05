using System;

namespace Democrachat.Db.Models
{
    public record Item
    {
        public int Id { get; init; }
        public int TemplateId { get; set; }
        public string Name { get; init; }
        public string Script { get; init; }
        public Guid PublicUuid { get; set; }
        public int OwnerId { get; set; }
        public string? ImageSrc { get; set; }
    }
}