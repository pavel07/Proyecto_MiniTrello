using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AddLaneResponseModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
    }
}