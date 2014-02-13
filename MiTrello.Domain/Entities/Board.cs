using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        private readonly IList<Account> _boards = new List<Account>();
        public virtual string Title { get; set; }

        public virtual Account Administrator { get; set; }
        public virtual long Id { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual IEnumerable<Account> Members { get { return _boards; } }

        public virtual void AddMember(Account member)
        {
            if (!_boards.Contains(member))
            {
                _boards.Add(member);
            }

        }
    }
}