using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class ChangePassModel
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmnewPassword { get; set; }
    }
}