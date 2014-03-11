using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MiniTrello.Api.Models
{
    public class AccountRegisterResponseModel
    {
        public AccountRegisterResponseModel(string Email, string FirstName, int CaseNumber)
        {
            this.FirstName = FirstName;
            if (CaseNumber == 1)
            {
                Message = ("Tu cuenta ha sido registrada exitosamente bajo el correo: " + Email);
            }
            else if (CaseNumber > 1)
            {
                Message = (FirstName + "! Lo sentimos mucho ya existe un usuario registrado con el correo " + Email);
            }
        }
        public string FirstName { get; set; }
        public string Message { get; set; }
    }
}