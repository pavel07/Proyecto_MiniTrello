using System;
using System.Collections.Generic;
using System.Linq;
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
    public class OrganizationController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public OrganizationController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [POST("organization/addBoard/{accesstoken}")]
        public HttpResponseMessage AddBoard(string accesstoken, [FromBody] AddBoardModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            var organization = _readOnlyRepository.GetById<Organization>(model.OrganizationId);
            var board = _mappingEngine.Map<AddBoardModel, Board>(model);
            board.Administrator = account;
            var boardcreated = _writeOnlyRepository.Create(board);
            organization.AddBoard(boardcreated);
            var organizationupdated = _writeOnlyRepository.Update(organization);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [AcceptVerbs("DELETE")]
        [DELETE("organization/removeboard")]
        public HttpResponseMessage RemoveBoard([FromBody] RemoveBoardModel model)
        {
            var board = _readOnlyRepository.GetById<Board>(model.BoardId);
            var archiveboard = _writeOnlyRepository.Archive(board);
            if (archiveboard.IsArchived)
            {
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.NotModified);
        }

        [AcceptVerbs("GET")]
        [GET("organization/{accesstoken}")]
        public List<GetOrganizationsModel> GetAllForUser(string accesstoken)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = _readOnlyRepository.GetById<Account>(sessions.User.Id);
            var mappedOrganizationModelList = _mappingEngine.Map<IEnumerable<Organization>,
                IEnumerable<GetOrganizationsModel>>(account.Organizations).ToList();
            var insArchivedList = new List<GetOrganizationsModel>();
            foreach (var orga in mappedOrganizationModelList)
            {
                if (orga.IsArchived == false)
                {
                    insArchivedList.Add(orga);
                }
            }
            return insArchivedList;
        } 

    }
}
