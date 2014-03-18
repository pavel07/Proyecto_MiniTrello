using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MiniTrello.Domain.Entities;
using MiniTrello.Api.Models;
using RestSharp;
using System.Net;

namespace MiniTrello.ApiWrapper
{
    public class MiniTrelloSdk
    {
        private static RestRequest InitRequest(string resource, Method method,object payload)
        {
            var request = new RestRequest(resource, method);
            request.AddHeader("Content-Type", "application/json");
            request.AddBody(payload);
            return request;
        }

        public static AuthenticationModel Login(AccountLoginModel loginModel)
        {
                var client = new RestClient(BaseUrl);
                var request = InitRequest("/login", Method.POST, loginModel);
                IRestResponse<AuthenticationModel> response = client.Execute<AuthenticationModel>(request);
                ConfigurationManager.AppSettings["accessToken"] = response.Data.Token;
                return response.Data;
        }

        public static AccountRegisterResponseModel Register(AccountRegisterModel registerModel)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("/register", Method.POST, registerModel);
            IRestResponse<AccountRegisterResponseModel> response = client.Execute<AccountRegisterResponseModel>(request);
            return response.Data;
        }

        public static AddOrganizationResponseModel AddOrganization(AddOrganizationModel model)
        {
            var client = new RestClient(BaseUrl);
            var resourceComplemetation = ("addorganization/{" + ConfigurationManager.AppSettings["accessToken"] + "}");
            var request = InitRequest(resourceComplemetation, Method.POST, model);
            IRestResponse<AddOrganizationResponseModel> response = client.Execute<AddOrganizationResponseModel>(request);
            return response.Data;
        }

        public static bool UpdateAccount(AccountUpdateModel model)
        {
            var client = new RestClient(BaseUrl);
            var resourceComplementation = ("{"+ConfigurationManager.AppSettings["accessToken"]+"}/updateprofile");
            var request = InitRequest(resourceComplementation, Method.PUT, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK");
        }

        public static bool restorePassword(ChangePassModel model)
        {
            var client = new RestClient(BaseUrl);
            var resourceComplementation = ("{" + ConfigurationManager.AppSettings["accessToken"] + "}/restorepassword");
            var request = InitRequest(resourceComplementation, Method.PUT, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK");
        }

        public static BoardModel AddMember(AddMemberBoardModel model)
        {
            var client = new RestClient(BaseUrl);
            var resourceComplemetation = ("boards/addmember/{" + ConfigurationManager.AppSettings["accessToken"] + "}");
            var request = InitRequest(resourceComplemetation, Method.PUT, model);
            IRestResponse<BoardModel> response = client.Execute<BoardModel>(request);
            return response.Data;
        }

        public static bool Renameboard(GetBoardModel model)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/renameboard", Method.PUT, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK");
        }

        public static bool AddLane(AddLaneModel model)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/addlane", Method.POST, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK");   
        }

        public string RemoveLane(RemoveLaneModel model)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("boards/removelane", Method.DELETE, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString();    
        }

        public MembersBoardModel showMembers(long boardId)
        {
            var client = new RestClient(BaseUrl);
            var resourceComplemetation = ("boards/getmembers/{" + boardId.ToString() + "}");
            var request = InitRequest(resourceComplemetation, Method.GET, null);
            IRestResponse<MembersBoardModel> response = client.Execute<MembersBoardModel>(request);
            return response.Data;
        }

        public static bool AddBoard(AddBoardModel model)
        {
            var client = new RestClient(BaseUrl);
            var resourceComplemetation = ("organization/addBoard/{" + ConfigurationManager.AppSettings["accessToken"] + "}");
            var request = InitRequest(resourceComplemetation, Method.POST, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK"); 
        }

        public static bool RemoveBoard(RemoveBoardModel model)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("organization/removeboard", Method.DELETE, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK"); 
        }

        public static bool AddCard(AddCardModel model)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("lanes/addcard", Method.POST, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK"); 
        }

        public static bool RemoveCard(RemoveCardModel model)
        {
            var client = new RestClient(BaseUrl);
            var request = InitRequest("lanes/removecard", Method.DELETE, model);
            IRestResponse<HttpResponse> response = client.Execute<HttpResponse>(request);
            return response.Data.StatusCode.ToString().Equals("OK");
        }
        private static string BaseUrl
        {
            get { return ConfigurationManager.AppSettings["baseUrl"]; }
        }
        
    }
}
