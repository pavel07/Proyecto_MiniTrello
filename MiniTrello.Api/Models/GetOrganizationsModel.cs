using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Models
{
    public class GetOrganizationsModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<AccountBoardModel> Boards { get; set; } 
    }
}