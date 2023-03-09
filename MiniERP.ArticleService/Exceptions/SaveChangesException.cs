namespace MiniERP.ArticleService.Exceptions
{
    public class SaveChangesException : Exception
    {
        public Type EntityType { get; set; }
        public SaveChangesException(Type entityType, string message) 
            : base(message) 
        {
            EntityType = entityType;
        }
    }
}
