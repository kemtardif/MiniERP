using CommonLib.Enums;

namespace CommonLib.Dtos
{
    public class ArticleEventDto : GenericEventDto
    {
        public int Id { get; set; }
        public ArticleType Type { get; set; }
        public bool HasInventory { get; set; }
    }
}
