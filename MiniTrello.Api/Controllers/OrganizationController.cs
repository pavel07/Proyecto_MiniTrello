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

        [POST("boards/{accesstoken}")]
        public AddBoardResponseModel AddBoard(string accesstoken, [FromBody] AddBoardModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                if (string.IsNullOrEmpty(model.Title))
                {
                    return new AddBoardResponseModel()
                    {
                        Title = "Error: ",
                        Message = "Titulo no puede estar vacio",
                        Status = 0
                    };
                }
                var organization = _readOnlyRepository.GetById<Organization>(model.organizationId);
                var board = _mappingEngine.Map<AddBoardModel, Board>(model);
                board.Administrator = account;
                var boardcreated = _writeOnlyRepository.Create(board);
                organization.AddBoard(boardcreated);
                _writeOnlyRepository.Update(organization);
                return new AddBoardResponseModel()
                {
                    Title = boardcreated.Title,
                    Message = "Board Creada Exitosamente",
                    Status = 2
                };
            }
            return new AddBoardResponseModel()
            {
                Title = "Error: ",
                Message = "No se pudo agregar la Board",
                Status = 0
            };
        }

        [AcceptVerbs("PUT")]
        [PUT("boards/{accesstoken}")]
        public RemoveBoardResponseModel RemoveBoard(string accesstoken,[FromBody] RemoveBoardModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var board = _readOnlyRepository.GetById<Board>(model.Id);
                _writeOnlyRepository.Archive(board);
                return new RemoveBoardResponseModel()
                {
                    Status = 2,
                    Message = "Se elimino exitosamente"
                };
            }
            return new RemoveBoardResponseModel()
            {
                Status = 0,
                Message = "No se pudo eliminar"
            };
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
