using Application.Dtos;
using Application.Features.Account.Commands;
using Application.Features.Account.Queries;
using Application.Features.Table.Commands;
using Application.Features.Table.Queries;
using AutoMapper;
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

        // table
        CreateMap<CreateTableCommand, Table>().ReverseMap();
        CreateMap<CreateTableCommandResponse, Table>().ReverseMap();
        CreateMap<GetTablesQueryResponse, Table>().ReverseMap();
        CreateMap<GetTableByNumberQueryResponse, Table>().ReverseMap();
        CreateMap<UpdateTableCommandResponse, Table>().ReverseMap();
        CreateMap<DeleteTableByNumberCommandResponse, Table>().ReverseMap();
    }
}