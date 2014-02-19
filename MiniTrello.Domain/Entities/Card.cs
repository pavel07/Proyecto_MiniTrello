﻿namespace MiniTrello.Domain.Entities
{
    public class Card: IEntity
    {
        public virtual long Id { get; set; }
        public virtual string Content { get; set; }
        public virtual bool IsArchived { get; set; }
    }
}
