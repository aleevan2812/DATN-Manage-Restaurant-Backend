using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using MediatR;

namespace Application.Features.Account.Queries;

public class GetEmployeesQueryResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Avatar { get; set; }
}

public class GetEmployeesQuery : IRequest<BaseResponse<List<GetEmployeesQueryResponse>>>
{
}

public class
    GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, BaseResponse<List<GetEmployeesQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GetEmployeesQueryResponse>>> Handle(GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var emps = _context.Accounts
            .Where(i => i.Role == Role.Employee)
            .OrderByDescending(i => i.CreatedAt)
            .ToList();

        var res = _mapper.Map<List<GetEmployeesQueryResponse>>(emps);

        return new BaseResponse<List<GetEmployeesQueryResponse>>(res, "Lấy danh sách nhân viên thành công");
    }
}