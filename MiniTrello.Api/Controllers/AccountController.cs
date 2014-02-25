using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using AttributeRouting.Helpers;
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
                string token = "Iniciar Sesion Nuevamente";
                var session =
                    _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.User.Email == account.Email)
                        .OrderByDescending(x => x.ExpirationTime)
                        .FirstOrDefault();
                if (session == null || session.ExpirationTime < DateTime.Now)
                {
                    Sessions newsession = SetSessionsModel(account);
                    Sessions newsessionCreated = _writeOnlyRepository.Create(newsession);
                    if (newsessionCreated != null)
                        token = newsessionCreated.Token;
                }
                else if (session.ExpirationTime > DateTime.Now)
                {
                    token = session.Token;
                }
                TimeSpan availabletime = (session.ExpirationTime.Subtract(DateTime.Now));
                return new AuthenticationModel()
                {
                    Token = token,
                    AvailableTime = availabletime
                };
            }
            throw new BadRequestException(
                "Usuario o clave incorrecto");
        }

        private Sessions SetSessionsModel(Account account)
        {
            var sessionLoging = new SessionsModel
            {
                User = account,
                LoginDate = DateTime.Now,
                ExpirationTime = DateTime.Now.AddHours(2),
                Token = Guid.NewGuid().ToString()
            };
            Sessions sessionToReturn = _mappingEngine.Map<SessionsModel, Sessions>(sessionLoging);
            return sessionToReturn;
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

        [POST("addorganization/{accesstoken}")]
        public HttpResponseMessage AddOrganization(string accesstoken, [FromBody] AddOrganizationModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var organization = _mappingEngine.Map<AddOrganizationModel, Organization>(model);
                var organizationCreated = _writeOnlyRepository.Create(organization); 
                account.AddOrganization(organizationCreated);
                var accountUpdated = _writeOnlyRepository.Update(account);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        [AcceptVerbs("PUT")]
        [PUT("{accesstoken}/updateprofile")]
        public HttpResponseMessage UpdateAccount(string accesstoken, [FromBody] AccountUpdateModel model)
        {
            Sessions sessions =
                   _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            account.FirstName = model.FirstName;
            account.LastName = model.LastName;
            account.Email = model.Email;
            _writeOnlyRepository.Update(account);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [AcceptVerbs("PUT")]
        [PUT("{accesstoken}/restorepassword")]
        public HttpResponseMessage restorePassword(string accesstoken, [FromBody] ChangePassModel model)
        {
            Sessions sessions =
                      _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account.Email != model.Email || model.newPassword != model.confirmnewPassword)
            {
                return new HttpResponseMessage(HttpStatusCode.NotModified);
            }
            account.Password = model.newPassword;
            _writeOnlyRepository.Update(account);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}