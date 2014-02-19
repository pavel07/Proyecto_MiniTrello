using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class MoveCardModel
    {
        public long OriginLaneId { get; set; }
        public long CardId { get; set; }
        public long DestinationLaneId { get; set; }

    }
}