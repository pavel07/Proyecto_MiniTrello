using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AddCardModel
    {
        public long LaneId { get; set; }
        public string Content { get; set; }
    }
}