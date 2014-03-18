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
        [PUT("boards/renameboard")]
        public HttpResponseMessage Renameboard([FromBody] GetBoardModel model)
        {
            Board board = _readOnlyRepository.GetById<Board>(model.BoardId);
            if (board != null)
            {
                board.Title = model.NewBoardTitle;
                board = _writeOnlyRepository.Update(board);
                return new HttpResponseMessage(HttpStatusCode.OK);    
            }
            return new HttpResponseMessage(HttpStatusCode.NotModified);
        }

        [POST("boards/addlane")]
        public HttpResponseMessage AddLane([FromBody] AddLaneModel model)
        {
            var lane = _mappingEngine.Map<AddLaneModel, Lane>(model);
            var lanecreated = _writeOnlyRepository.Create(lane);
            var board = _readOnlyRepository.GetById<Board>(model.BoardId);
            if (board != null)
            {
                board.AddLane(lanecreated);
                _writeOnlyRepository.Update(board);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [AcceptVerbs("DELETE")]
        [DELETE("boards/removelane")]
        public HttpResponseMessage RemoveLane([FromBody] RemoveLaneModel model)
        {
            var lane = _readOnlyRepository.GetById<Lane>(model.LaneId);
            var archivelane = _writeOnlyRepository.Archive(lane);
            if (archivelane.IsArchived)
            {
                return  new HttpResponseMessage(HttpStatusCode.OK);
            }
            return new HttpResponseMessage(HttpStatusCode.NotModified);
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

    }
}
