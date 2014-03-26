using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AccountUpdateModel
    {
        public string NewFirstName { get; set; }
        public string NewLastName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}