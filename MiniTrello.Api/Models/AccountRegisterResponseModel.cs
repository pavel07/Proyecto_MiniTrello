﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AccountRegisterResponseModel
    {
        public AccountRegisterResponseModel(string Email, string FirstName)
        {
            this.FirstName = FirstName;
            Message = ("Tu cuenta ha sido registrada exitosamente bajo el correo: " + Email);
        }
        public string FirstName { get; set; }
        public string Message { get; set; }
    }
}