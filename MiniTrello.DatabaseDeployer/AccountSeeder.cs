using System.Collections.Generic;
using DomainDrivenDatabaseDeployer;
using FizzWare.NBuilder;
using MiniTrello.Domain.Entities;
using NHibernate;

namespace MiniTrello.DatabaseDeployer
{
    public class AccountSeeder : IDataSeeder
    {
        readonly ISession _session;

        public AccountSeeder(ISession session)
        {
            _session = session;
        }

        public void Seed()
        {
            IList<Account> accountList = Builder<Account>.CreateListOfSize(2).Build();
            foreach (Account account in accountList)
            {   
                var boards = Builder<Board>.CreateListOfSize(2).Build();
                boards[0].Title = "Welcome Board";
                boards[1].Title = "Second Board";
                foreach (var board in boards)
                {
                    var lanes = Builder<Lane>.CreateListOfSize(3).Build();
                    lanes[0].Title = "To Do";
                    lanes[1].Title = "Doing";
                    lanes[2].Title = "Done";
                    foreach (var lane in lanes)
                    {
                        _session.Save(lane);
                    }
                    board.AddLane(lanes[0]);
                    board.AddLane(lanes[1]);
                    board.AddLane(lanes[2]);
                    //board.Administrator = account;
                    _session.Save(board);
                }

                var organization = new Organization();
                organization.Title = "My Boards";
                organization.AddBoard(boards[0]);
                organization.AddBoard(boards[1]);
                _session.Save(organization);

                account.AddOrganization(organization);
                _session.Save(account);
                foreach (var board in boards)
                {
                    board.Administrator = account;
                    _session.Update(board);
                }
            }
        }
    }
}