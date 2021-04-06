namespace Democrachat.Db.Models
{
    public record KudoListing
    {
        public int TemplateId;
        public int Weight;

        public KudoListing()
        {
        }

        public KudoListing(int templateId, int weight)
        {
            TemplateId = templateId;
            Weight = weight;
        }
    }
}