using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using AttributeRouting.Web.Http;
using AutoMapper;
using Microsoft.Ajax.Utilities;
using MiniTrello.Api.CustomExceptions;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;
using MiniTrello.Domain.Services;

namespace MiniTrello.Api.Controllers
{
    public class BoardController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public BoardController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [AcceptVerbs("PUT")]
        [PUT("boards/addmember/{accessToken}")]
        public BoardModel AddMember([FromBody] AddMemberBoardModel model, string accessToken)
        {
            //validar seguridad
            var memberToAdd = _readOnlyRepository.GetById<Account>(model.MemberId);
            var board = _readOnlyRepository.GetById<Board>(model.BoardId);
            
            board.AddMemberAccount(memberToAdd);
            var updatedBoard = _writeOnlyRepository.Update(board);
            var boardModel = _mappingEngine.Map<Board, BoardModel>(updatedBoard);
            return boardModel;
        }

        [AcceptVerbs("PUT")]
        [PUT("boards/renameboard/{accesstoken}")]
        public RenameBoardResponseModel Renameboard([FromBody] RenameBoardModel model, string accesstoken)
        {
            Sessions sessions =
               _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var board = _readOnlyRepository.GetById<Board>(model.Id);
                if (string.IsNullOrEmpty(model.NewTitle))
                {
                    return new RenameBoardResponseModel()
                    {
                        Status = 0,
                        Message = "Error: Titulo no puede estar vacio"
                    };
                }
                board.Title = model.NewTitle;
                var boardUpdated = _writeOnlyRepository.Update(board);
                return new RenameBoardResponseModel()
                {
                    Status = 2,
                    Message = "Listo"
                };
            }
            return new RenameBoardResponseModel()
            {
                Status = 0,
                Message = "Error"
            };
        }

        [POST("lanes/{accesstoken}")]
        public AddLaneResponseModel AddLane(string accesstoken, [FromBody] AddLaneModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                if (string.IsNullOrEmpty(model.Title))
                {
                    return new AddLaneResponseModel()
                    {
                        Title = "Error: ",
                        Message = "Titulo no puede estar vacio",
                        Status = 0
                    };
                }
                var board = _readOnlyRepository.GetById<Board>(model.BoardId);
                var lane = _mappingEngine.Map<AddLaneModel, Lane>(model);
                var lanecreated = _writeOnlyRepository.Create(lane);
                board.AddLane(lanecreated);
                _writeOnlyRepository.Update(board);
                return new AddLaneResponseModel()
                {
                    Title = lanecreated.Title,
                    Message = "Lane Creada Exitosamente",
                    Status = 2
                };
            }
            return new AddLaneResponseModel()
            {
                Title = "Error: ",
                Message = "No se pudo agregar la Board",
                Status = 0
            };
        }

        [AcceptVerbs("PUT")]
        [PUT("lanes/removelane/{accesstoken}")]
        public RemoveBoardResponseModel RemoveLane(string accesstoken, [FromBody] RemoveBoardModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var lane = _readOnlyRepository.GetById<Lane>(model.Id);
                _writeOnlyRepository.Archive(lane);
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
        [GET("boards/getmembers/{boardId}")]
        public MembersBoardModel showMembers(long boardId )
        {
            var board = _readOnlyRepository.GetById<Board>(boardId);
            var members = new MembersBoardModel();

            foreach (var account in board.Accounts)
            {
                members._MembersList.Add(account.FirstName);
            }

            return members;
        }

        [AcceptVerbs("GET")]
        [GET("boards/{organizationId}/{accesstoken}")]
        public List<GetBoardsModel> GetAllBoardsForOrganization(long organizationId,string accesstoken)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = _readOnlyRepository.GetById<Account>(sessions.User.Id);
            
            var organization = _readOnlyRepository.GetById<Organization>(organizationId);
            var mappedBoardModelList = _mappingEngine.Map<IEnumerable<Board>,
                IEnumerable<GetBoardsModel>>(organization.Boards).ToList();
            var insArchivedList = new List<GetBoardsModel>();
            foreach (var boar in mappedBoardModelList)
            {
                if (boar.IsArchived == false)
                {
                    insArchivedList.Add(boar);
                }
            }
            return insArchivedList;
        } 
    }
}
