namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        public virtual string Title { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }
    }
}