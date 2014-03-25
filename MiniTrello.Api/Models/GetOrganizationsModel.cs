using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Models
{
    public class GetOrganizationsModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsArchived { get; set; }
    }
}