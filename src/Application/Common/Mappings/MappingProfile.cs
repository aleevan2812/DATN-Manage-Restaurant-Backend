using Application.Features.Account.Commands;
using Application.Features.Account.Queries;
using Application.Features.Dish;
using Application.Features.Guest;
using Application.Features.Order;
using Application.Features.Table.Commands;
using Application.Features.Table.Queries;
using AutoMapper;
using Core.Dtos;
using Core.Entities;

namespace Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // acount
        CreateMap<AccountDto, Account>().ReverseMap();
        CreateMap<GetUserProfileResponse, Account>().ReverseMap();
        CreateMap<UpdateMeCommandResponse, Account>().ReverseMap();
        CreateMap<CreateEmployeeCommand, Account>().ReverseMap();
        CreateMap<CreateEmployeeCommandResponse, Account>().ReverseMap();
        CreateMap<GetEmployeesQueryResponse, Account>().ReverseMap();
        CreateMap<GetDetailAccountQueryResponse, Account>().ReverseMap();
        CreateMap<UpdateEmployeeDetailCommandResponse, Account>().ReverseMap();
        CreateMap<DeleteEmployeeCommandResponse, Account>().ReverseMap();
        CreateMap<ChangePasswordCommandResponse, Account>().ReverseMap();

        CreateMap<AccountResponse, Account>().ReverseMap();

        // table
        CreateMap<CreateTableCommand, Table>().ReverseMap();
        CreateMap<CreateTableCommandResponse, Table>().ReverseMap();
        CreateMap<GetTablesQueryResponse, Table>().ReverseMap();
        CreateMap<GetTableByNumberQueryResponse, Table>().ReverseMap();
        CreateMap<UpdateTableCommandResponse, Table>().ReverseMap();
        CreateMap<DeleteTableByNumberCommandResponse, Table>().ReverseMap();
        CreateMap<Table, TableResponse>().ReverseMap();
        ;

        // guest 
        CreateMap<LoginGuestCommand, Guest>().ReverseMap();
        CreateMap<GuestDto, Guest>().ReverseMap();
        CreateMap<Guest, GetGuestsByDateQueryResponse>().ReverseMap();

        // dish
        CreateMap<CreateDishCommand, Dish>().ReverseMap();
        CreateMap<CreateDishCommandResponse, Dish>().ReverseMap();
        CreateMap<GetDishesQueryResponse, Dish>().ReverseMap();
        CreateMap<GetDishByIdQueryResponse, Dish>().ReverseMap();
        CreateMap<DeleteDishByIdCommandResponse, Dish>().ReverseMap();
        CreateMap<UpdateDishByIdCommand, Dish>().ReverseMap();
        CreateMap<UpdateDishByIdCommandResponse, Dish>().ReverseMap();

        // dishSnapshot
        CreateMap<DishSnapshot, Dish>().ReverseMap();
        CreateMap<DishSnapshot, DishSnapshotResponse>().ReverseMap();
        CreateMap<GuestInforResponse, Guest>().ReverseMap();

        // order
        CreateMap<Order, GuestCreateOrderCommandResponse>();
        // .ForMember(dest => dest.Guest, opt => opt.Ignore())
        // .ForMember(dest => dest.DishSnapshot, opt => opt.Ignore())
        // .ForMember(dest => dest.OrderHandler, opt => opt.Ignore());
        CreateMap<Order, GetOrdersByDateQueryResponse>().ReverseMap();
        CreateMap<Order, GetOrderByIdQueryResponse>().ReverseMap();
    }
}