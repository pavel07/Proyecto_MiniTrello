namespace MiTrello.Domain.Entities
{
    public class Board : IEntity
    {
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual string Title { get; set; }
    }
}