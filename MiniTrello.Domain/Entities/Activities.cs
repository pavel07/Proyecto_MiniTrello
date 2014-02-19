using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    class Activities: IEntity
    {
        public virtual long Id { get; set; }
        public virtual Account accountPoster { get; set; }
        public virtual bool IsArchived { get; set; }
    }
}
