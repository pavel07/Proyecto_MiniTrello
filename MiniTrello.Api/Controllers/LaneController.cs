﻿using System;
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
    public class LaneController : ApiController
    {
        readonly IReadOnlyRepository _readOnlyRepository;
        readonly IWriteOnlyRepository _writeOnlyRepository;
        readonly IMappingEngine _mappingEngine;

        public LaneController(IReadOnlyRepository readOnlyRepository, IWriteOnlyRepository writeOnlyRepository,
            IMappingEngine mappingEngine)
        {
            _readOnlyRepository = readOnlyRepository;
            _writeOnlyRepository = writeOnlyRepository;
            _mappingEngine = mappingEngine;
        }

        [POST("lanes/addcard/{accesstoken}")]
        public AddLaneResponseModel AddCard(string accesstoken, [FromBody] AddCardModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                if (string.IsNullOrEmpty(model.Content))
                {
                    return new AddLaneResponseModel()
                    {
                        Title = "Error: ",
                        Message = "Contenido Vacio",
                        Status = 0
                    };
                }
                var lane = _readOnlyRepository.GetById<Lane>(model.LaneId);
                var card = _mappingEngine.Map<AddCardModel, Card>(model);
                var cardcreated = _writeOnlyRepository.Create(card);
                lane.AddCard(cardcreated);
                _writeOnlyRepository.Update(lane);
                return new AddLaneResponseModel()
                {
                    Title = "",
                    Message = "Card Agregada",
                    Status = 2
                };
            }
            return new AddLaneResponseModel()
            {
                Title = "Error: ",
                Message = "No se pudo agregar la Card",
                Status = 0
            };
        }

        [AcceptVerbs("PUT")]
        [PUT("lanes/removecard/{accesstoken}")]
        public RemoveBoardResponseModel RemoveCard(string accesstoken, [FromBody] RemoveCardModel model)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var card = _readOnlyRepository.GetById<Card>(model.CardId);
                _writeOnlyRepository.Archive(card);
                return new RemoveBoardResponseModel()
                {
                    Status = 2,
                    Message = "Card Eliminada"
                };
            }
            return new RemoveBoardResponseModel()
            {
                Status = 0,
                Message = "No se pudo eliminar"
            };
        }
       
        [AcceptVerbs("PUT")]
        [PUT("lanes/movecard")]
        public HttpResponseMessage MoveCardtoOtherLane([FromBody] MoveCardModel model)
        {
            var originlane = _readOnlyRepository.GetById<Lane>(model.OriginLaneId);
            var cardtomove = _readOnlyRepository.GetById<Card>(model.CardId);
            var destinationlane = _readOnlyRepository.GetById<Lane>(model.DestinationLaneId);

            destinationlane.AddCard(cardtomove);
            originlane.RemoveCard(cardtomove);

            _writeOnlyRepository.Update(originlane);
            _writeOnlyRepository.Update(destinationlane);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [AcceptVerbs("PUT")]
        [PUT("lanes/renamelane/{accesstoken}")]
        public RenameBoardResponseModel Renameboard([FromBody] RenameBoardModel model, string accesstoken)
        {
            Sessions sessions =
               _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = sessions.User;
            if (account != null)
            {
                var lane = _readOnlyRepository.GetById<Lane>(model.Id);
                if (string.IsNullOrEmpty(model.NewTitle))
                {
                    return new RenameBoardResponseModel()
                    {
                        Status = 0,
                        Message = "Error: Titulo no puede estar vacio"
                    };
                }
                lane.Title = model.NewTitle;
                var boardUpdated = _writeOnlyRepository.Update(lane);
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

        [AcceptVerbs("GET")]
        [GET("lanes/{boardId}/{accesstoken}")]
        public List<GetLanesModel> GetAllLanesForBoard(long boardId, string accesstoken)
        {
            Sessions sessions =
                _readOnlyRepository.Query<Sessions>(sessions1 => sessions1.Token == accesstoken).FirstOrDefault();
            Account account = _readOnlyRepository.GetById<Account>(sessions.User.Id);

            var board = _readOnlyRepository.GetById<Board>(boardId);
            var mappedLaneModelList = _mappingEngine.Map<IEnumerable<Lane>,
                IEnumerable<GetLanesModel>>(board.Lanes).ToList();
            foreach (var lane in mappedLaneModelList)
            {
                lane.Cards = GetCardsForLane(accesstoken,lane.Id);
               
            }
            return mappedLaneModelList.Where(lane => lane.IsArchived == false).ToList();
        }

        [AcceptVerbs("GET")]
        [GET("lanes/{laneId}/{accesstoken}")]
        public List<CardModel> GetCardsForLane(string accesstoken, long laneId)
        {
            
            var lane = _readOnlyRepository.GetById<Lane>(laneId);
            var cardList = new List<CardModel>();
           
            if (lane != null)
            {
                var mappedCardModelList = _mappingEngine.Map<IEnumerable<Card>, IEnumerable<CardModel>>(lane.Cards);
                cardList = mappedCardModelList.Where(card => !card.IsArchived).ToList();
            }
            return cardList;
        }
    }
}
