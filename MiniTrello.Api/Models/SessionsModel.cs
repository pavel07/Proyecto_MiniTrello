using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MiniTrello.Domain.Entities;

namespace MiniTrello.Api.Models
{
    public class SessionsModel
    {
        public Account User { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime ExpirationTime { get; set; }
        public string Token { get; set; }

    }
}