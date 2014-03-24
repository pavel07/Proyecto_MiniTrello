using System.Collections.Generic;
using System.Configuration;

namespace MiniTrello.Domain.Entities
{
    public class Organization : IEntity
    {
        private readonly IList<Board> _boards = new List<Board>();
        public virtual long Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual bool IsArchived { get; set; }
        public virtual IEnumerable<Board> Board
        {
            get { return _boards; }
            set { }
        }

        public virtual void AddBoard(Board board)
        {
            if (!_boards.Contains(board))
            {
                _boards.Add(board);
            }
        }
    }
}