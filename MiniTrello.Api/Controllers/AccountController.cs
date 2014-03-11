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
using MiniTrello.Api.Controllers.AccountControllerHelpers;
using RestSharp;

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

        [HttpPost]
        [POST("login")]
        public AuthenticationModel Login([FromBody] AccountLoginModel model)
        {
            var encryptObj = new EncryptServices();
            var account =
                _readOnlyRepository.First<Account>(
                    account1 => account1.Email == model.Email);
            if (account.Password !=
                encryptObj.EncryptStringToBytes(model.Password, account.EncryptKey, account.EncryptIV))
            {
                throw new BadRequestException(
                "Clave incorrecta: " + encryptObj.EncryptStringToBytes(model.Password, account.EncryptKey, account.EncryptIV));
            }

            if (account != null)
            {
                string token = "";
                TimeSpan availabletime = new TimeSpan();
                var session =
                    _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.User.Email == account.Email)
                        .OrderByDescending(x => x.ExpirationTime)
                        .FirstOrDefault();
                if (session == null || session.ExpirationTime < DateTime.Now)
                {
                    Sessions newsession = SetSessionsModel(account);
                    Sessions newsessionCreated = _writeOnlyRepository.Create(newsession);
                    if (newsessionCreated != null)
                    {
                        token = newsessionCreated.Token;
                        availabletime = (newsessionCreated.ExpirationTime.Subtract(newsessionCreated.LoginDate));
                    }
                    else
                    {
                        throw new BadRequestException("No se pudo crear la Session de Login");
                    }
                }
                else if (session.ExpirationTime > DateTime.Now)
                {
                    token = session.Token;
                    availabletime = (session.ExpirationTime.Subtract(DateTime.Now));
                }
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
        public AccountRegisterResponseModel Register([FromBody] AccountRegisterModel model)
        {   
            var validateMessage = _registerValidator.Validate(model);
            if (!String.IsNullOrEmpty(validateMessage))
            {
                throw new BadRequestException(validateMessage);
            }
            var accountExist =
                _readOnlyRepository.First<Account>(
                    account1 => account1.Email == model.Email);
            if (accountExist == null)
            {
                Account account = _mappingEngine.Map<AccountRegisterModel, Account>(model);
                var encryptObj = new EncryptServices();
                encryptObj.GenerateKeys();
                account.Password = encryptObj.EncryptStringToBytes(account.Password, encryptObj.myRijndael.Key,
                    encryptObj.myRijndael.IV);
                account.EncryptKey = encryptObj.myRijndael.Key;
                account.EncryptIV = encryptObj.myRijndael.IV;
                Account accountCreated = _writeOnlyRepository.Create(account);
                if (accountCreated != null)
                {
                    SendSimpleMessage(accountCreated.FirstName, accountCreated.LastName, accountCreated.Email);
                    return new AccountRegisterResponseModel(accountCreated.Email, accountCreated.FirstName, 1);
                }
                throw new BadRequestException("Hubo un error al guardar el usuario");
            }
            return new AccountRegisterResponseModel(model.Email, model.FirstName, 2);
        }

        private static RestResponse SendSimpleMessage(string FirstName, string LastName, string Email)
        {
            RestClient client = new RestClient();
            client.BaseUrl = "https://api.mailgun.net/v2";
            client.Authenticator =
                   new HttpBasicAuthenticator("api",
                                              "key-64xe-jjly8m-3whcyvnm9fr2ivbjqel7");
            RestRequest request = new RestRequest();
            request.AddParameter("domain",
                                "app13172.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "MiniTrello Web <postmaster@app13172.mailgun.org>");
            request.AddParameter("to", FirstName + " " + LastName +" <"+Email+">");
            request.AddParameter("subject", "Thank You for Sign Up | MiniTrello Web");
            request.AddParameter("text", "Congratulations " + FirstName + ", you just has Sign Up in MiniTrello Web, go to Login Page and enjoy all ours Features.-");
            request.Method = Method.POST;
            return (RestResponse) client.Execute(request);
        }

        [POST("addorganization/{accesstoken}")]
        public AddOrganizationResponseModel AddOrganization(string accesstoken, [FromBody] AddOrganizationModel model)
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
                return new AddOrganizationResponseModel(organizationCreated.Title, "Organizacion Creada Exitosamente");
            }
            return new AddOrganizationResponseModel("Error:","No se pudo agregar la Organizacion");
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