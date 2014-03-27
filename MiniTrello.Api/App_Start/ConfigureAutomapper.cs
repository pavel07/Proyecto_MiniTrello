using System;
using AutoMapper;
using MiniTrello.Api.Models;
using MiniTrello.Domain.Entities;
using MiniTrello.Infrastructure;

namespace MiniTrello.Api
{
    public class ConfigureAutomapper : IBootstrapperTask
    {
        public void Run()
        {
            Mapper.CreateMap<Account, AccountLoginModel>().ReverseMap();
            Mapper.CreateMap<Account, AccountRegisterModel>().ReverseMap();
            Mapper.CreateMap<Account, AccountRegisterResponseModel>().ReverseMap();
            Mapper.CreateMap<Organization, AddOrganizationModel>().ReverseMap();
            Mapper.CreateMap<Organization, GetOrganizationsModel>().ReverseMap();
            Mapper.CreateMap<Board, GetBoardsModel>().ReverseMap();
            Mapper.CreateMap<Lane, GetLanesModel>().ReverseMap();
            Mapper.CreateMap<RemoveOrganizationModel, Organization>().ReverseMap();
            Mapper.CreateMap<Account, GetOrganizationsModel>().ReverseMap();
            Mapper.CreateMap<Lane, AddLaneModel>().ReverseMap();
            Mapper.CreateMap<Card, AddCardModel>().ReverseMap();
            Mapper.CreateMap<Card, CardModel>().ReverseMap();
            Mapper.CreateMap<Board, BoardModel>().ReverseMap();
            Mapper.CreateMap<Board, AddBoardModel>().ReverseMap();
            Mapper.CreateMap<Sessions, SessionsModel>().ReverseMap();
            //Mapper.CreateMap<DemographicsEntity, DemographicsModel>().ReverseMap();
            //Mapper.CreateMap<IReportEntity, IReportModel>()
            //    .Include<DemographicsEntity, DemographicsModel>();
        }
    }
}