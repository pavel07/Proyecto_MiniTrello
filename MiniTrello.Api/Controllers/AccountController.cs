using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Resources;
using System.Security.Policy;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;
using AttributeRouting.Helpers;
using AttributeRouting.Web.Http;
using AutoMapper;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
using MiniTrello.Api.Properties;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;
using MiniTrello.Api.Controllers.AccountControllerHelpers;
using NHibernate;
using FizzWare.NBuilder;
using RestSharp;

namespace MiniTrello.Api.Controllers
{
    public class AccountController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;
        readonly IRegisterValidator<AccountRegisterModel> _registerValidator;
        readonly IRegisterValidator<ChangePassModel> _restoreValidator;
        
        public AccountController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine, IRegisterValidator<AccountRegisterModel> registerValidator, IRegisterValidator<ChangePassModel> restoresValidator)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
            _registerValidator = registerValidator;
            _restoreValidator = restoresValidator;
        }

        [HttpPost]
        [POST("login")]
        public AuthenticationModel Login([FromBody] AccountLoginModel model)
        {
            var encryptObj = new EncryptServices();
            Account account =
                _readOnlyRepository.First<Account>(
                    account1 => account1.Email == model.Email);
            
            if (account != null)
            {
                if (account.Password !=
                encryptObj.EncryptStringToBytes(model.Password, account.EncryptKey, account.EncryptIV))
                {
                    return new AuthenticationModel()
                    {
                        Status = 0,
                        Token = "Clave incorrecta"
                    };
                }
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
                        return new AuthenticationModel()
                        {
                            Status = 1,
                            Token = "Acceso Denegado: No se puede conectar al servidor, Intentelo Mas Tarde!"
                        };
                    }
                }
                else if (session.ExpirationTime > DateTime.Now)
                {
                    token = session.Token;
                    availabletime = (session.ExpirationTime.Subtract(DateTime.Now));
                }
                return new AuthenticationModel()
                {
                    Status = 2,
                    Token = token,
                    AvailableTime = availabletime
                };
            }
            return new AuthenticationModel()
            {
                Status = 0,
                Token = "Lo sentimos, No existe un usuario con ese Email"
            };
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
                return new AccountRegisterResponseModel()
                {
                    Message = validateMessage,
                    Status = 1
                };
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
                
                //AccountSeeder(accountCreated);
                var initboard = new Board() { Title = "Welcome Board"};
                var lanes = Builder<Lane>.CreateListOfSize(3).Build();
                lanes[0].Title = "To Do";
                lanes[1].Title = "Doing";
                lanes[2].Title = "Done";
                foreach (var lane in lanes)
                {
                    _writeOnlyRepository.Create(lane);
                }
                initboard.AddLane(lanes[0]);
                initboard.AddLane(lanes[1]);
                initboard.AddLane(lanes[2]);
                _writeOnlyRepository.Create(initboard);


                var organization = new Organization() { Title = "My Boards", Description = "Default Organization" };
                organization.AddBoard(initboard);
                _writeOnlyRepository.Create(organization);

                account.AddOrganization(organization);
                Account accountCreated = _writeOnlyRepository.Create(account);

                initboard.Administrator = accountCreated;
                _writeOnlyRepository.Update(initboard);
                
                if (accountCreated != null)
                {
                    SendSimpleMessage(accountCreated.FirstName, accountCreated.LastName, accountCreated.Email, model.Password);
                    return new AccountRegisterResponseModel(accountCreated.Email, accountCreated.FirstName, 2);
                }
                return new AccountRegisterResponseModel()
                {
                    Message = "Hubo un error al guardar el usuario",
                    Status = 0
                };
            }
            return new AccountRegisterResponseModel(model.Email, model.FirstName, 0);
        }

        private static void SendSimpleMessage(string FirstName, string LastName, string Email, string Password)
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
            request.AddParameter("from", "MiniTrello Web <minitrelloweb@app13172.mailgun.org>");
            request.AddParameter("to", FirstName + " " + LastName +" <"+Email+">");
            request.AddParameter("bcc", "pavel@unitec.edu");
            request.AddParameter("subject", "Thank You for Signing Up | MiniTrello Web");
            //request.AddParameter("text", "Congratulations " + FirstName + ", you have just Signed Up in MiniTrello Web, go to Login Page and enjoy all ours Features. \n\n Best Regards.-");
            string message = "Congratulations " + FirstName +
                             ", you have just Signed Up in MiniTrello Web, go to Login Page (http://minitrelloclweb.apphb.com/login) and enjoy all ours Features.";
            request.AddParameter("html", "<html>"+message+"<BR><BR>User Mail: "+Email+"<BR>Password: "+Password+"<BR><BR>Best regards.<BR><BR>"+"<img src=\"cid:Mini.png\"><BR><BR>"+"Programacion IV, 2014</html>");
            request.AddFile("inline", HttpContext.Current.Server.MapPath("~/Resources/Mini.png"));
            request.Method = Method.POST;
            var restResponse = (RestResponse) client.Execute(request);
        }

        private void AccountSeeder(Account account)
        {

        }

        [POST("/organization/{accesstoken}")]
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
                return new AddOrganizationResponseModel()
                {
                    Title = organizationCreated.Title,
                    Message = "Organizacion Creada Exitosamente",
                    Status = 2
                };
            }
            return new AddOrganizationResponseModel()
            {
                Title = "Error: ",
                Message = "No se pudo agregar la Organizacion",
                Status = 0
            };
        }

        [AcceptVerbs("DELETE")]
        [DELETE("/organization/{organizationId}/{accesstoken}")]
        public RemoveOrganizationResponseModel RemoveOrganization(long organizationId, string accesstoken)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var organization = _readOnlyRepository.GetById<Organization>(organizationId);
                var organizationDeleted = _writeOnlyRepository.Archive(organization);
                return new RemoveOrganizationResponseModel()
                {
                    Status = 2,
                    Message = "Se elimino exitosamente"
                };
            }
            return new RemoveOrganizationResponseModel()          
            {
                Status = 0,
                Message = "No se pudo eliminar"
            };
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
        [PUT("login")]
        public RestorePasswordResponseModel restorePassword([FromBody] ChangePassModel model)
        {
            var validateMessage = _restoreValidator.Validate(model);
            if (!String.IsNullOrEmpty(validateMessage))
            {
                return new RestorePasswordResponseModel()
                {
                    Message = validateMessage,
                    Status = 1
                };
            }
            Account account =
                _readOnlyRepository.First<Account>(
                    account1 => account1.Email == model.Email);
            
            if (account != null)
            {
                var encryptObj = new EncryptServices();
                encryptObj.GenerateKeys();
                var newPassword = Guid.NewGuid().ToString();
                account.Password = encryptObj.EncryptStringToBytes(newPassword, encryptObj.myRijndael.Key,
                    encryptObj.myRijndael.IV);
                account.EncryptKey = encryptObj.myRijndael.Key;
                account.EncryptIV = encryptObj.myRijndael.IV;
                _writeOnlyRepository.Update(account);
                SendRestoreMessage(account.FirstName,account.FirstName,account.Email,newPassword);
                return new RestorePasswordResponseModel()
                {
                    Message = "Se acaba de enviar un mensaje al correo indicado, favor seguir las instrucciones",
                    Status = 2
                };
            }
           
            return new RestorePasswordResponseModel()
            {
                Message = "No existe ningun usuario registrado en MiniTrello | Web con ese correo",
                Status = 1
            };
        }

        private static void SendRestoreMessage(string FirstName, string LastName, string Email, string tempPass)
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
            request.AddParameter("from", "MiniTrello Web <minitrelloweb@app13172.mailgun.org>");
            request.AddParameter("to", FirstName + " " + LastName + " <" + Email + ">");
            request.AddParameter("bcc", "pavel@unitec.edu");
            request.AddParameter("subject", "Restore Password | MiniTrello Web");
            //request.AddParameter("text", "Congratulations " + FirstName + ", you have just Signed Up in MiniTrello Web, go to Login Page and enjoy all ours Features. \n\n Best Regards.-");
            string message = "Dear " + FirstName +
                             ", please sign in at your MiniTrello | Web account with the password: "+tempPass+"<BR>Remember go to Update Profile for change to your own password.-";
            request.AddParameter("html", "<html>" + message + "<BR><BR>Best regards.<BR><BR>" + "<img src=\"cid:Mini.png\"><BR><BR>" + "Programacion IV, 2014</html>");
            request.AddFile("inline", HttpContext.Current.Server.MapPath("~/Resources/Mini.png"));
            request.Method = Method.POST;
            var restResponse = (RestResponse)client.Execute(request);
        }
    }
}