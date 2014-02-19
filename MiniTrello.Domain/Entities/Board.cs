using System.Collections.Generic;

namespace MiniTrello.Domain.Entities
{
    public class Board : IEntity
    {
        private readonly IList<Account> _membersAccounts = new List<Account>();
        private readonly IList<Lane> _lanes = new List<Lane>();
        private readonly IList<Activities> _activitieses = new List<Activities>(); 
        public virtual long Id { get; set; }
        public virtual string Title { get; set; }
        public virtual Account Administrator { get; set; }
        public virtual bool IsArchived { get; set; }

        public virtual IEnumerable<Account> Accounts
        {
            get { return _membersAccounts; }
            set { }
        }
        public virtual IEnumerable<Lane> Lanes
        {
            get { return _lanes; }
            set { }
        }
        public virtual void AddMemberAccount (Account memberAccount)
        {
            if (!_membersAccounts.Contains(memberAccount))
            {
                _membersAccounts.Add(memberAccount);
            }
        }
        public virtual void AddLane(Lane lane)
        {
            if (!_lanes.Contains(lane))
            {
                _lanes.Add(lane);    
            }
        }

        public virtual void RenameBoard(string newTitle)
        {
            Title = newTitle;
        }
    }
}