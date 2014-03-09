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
            Mapper.CreateMap<Lane, AddLaneModel>().ReverseMap();
            Mapper.CreateMap<Card, AddCardModel>().ReverseMap();
            Mapper.CreateMap<Board, BoardModel>().ReverseMap();
            Mapper.CreateMap<Board, AddBoardModel>().ReverseMap();
            Mapper.CreateMap<Sessions, SessionsModel>().ReverseMap();
            //Mapper.CreateMap<DemographicsEntity, DemographicsModel>().ReverseMap();
            //Mapper.CreateMap<IReportEntity, IReportModel>()
            //    .Include<DemographicsEntity, DemographicsModel>();
        }
    }
}