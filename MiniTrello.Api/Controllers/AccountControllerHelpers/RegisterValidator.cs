using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers.AccountControllerHelpers
{
    public class RegisterValidator : IRegisterValidator<AccountRegisterModel>
    {
        public string Validate(AccountRegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                return "Claves no son iguales";
            }
            return "";
        }

    }
}