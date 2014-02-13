using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers
{
    public class AccountController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;
        readonly IRegisterValidator<AccountRegisterModel> _registerValidator;

        public AccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine, IRegisterValidator<AccountRegisterModel> registerValidator)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
            _registerValidator = registerValidator;
        }

        [POST("login")]
        public AuthenticationModel Login([FromBody] AccountLoginModel model)
        {
            var account =
                _readOnlyRepository.First<Account>(
                    account1 => account1.Email == model.Email && account1.Password == model.Password);
            if (account != null)
            {
                return new AuthenticationModel(){Token = "Existe"};
            }
            
            throw new BadRequestException(
                "Usuario o clave incorrecto");
        }

        [POST("register")]
        public HttpResponseMessage Register([FromBody] AccountRegisterModel model)
        {
            
            var validateMessage = _registerValidator.Validate(model);
            if (!String.IsNullOrEmpty(validateMessage))
            {
                throw new BadRequestException(validateMessage);
            }
            Account account = _mappingEngine.Map<AccountRegisterModel, Account>(model);
            Account accountCreated = _writeOnlyRepository.Create(account);
            if (accountCreated != null)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            throw new BadRequestException("Hubo un error al guardar el usuario");
        }
    }
}