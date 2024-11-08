using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries;

public class GetDetailAccountQueryResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Avatar { get; set; }
}

public class GetDetailAccountQuery : IRequest<BaseResponse<GetDetailAccountQueryResponse>>
{
    public GetDetailAccountQuery(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}

public class
    GetDetailAccountQueryHandler : IRequestHandler<GetDetailAccountQuery,
    BaseResponse<GetDetailAccountQueryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDetailAccountQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetDetailAccountQueryResponse>> Handle(GetDetailAccountQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(i => i.Id == request.Id);
        if (account == null) throw new NotFoundException("Không tìm thấy thông tin nhân viên !");

        var res = _mapper.Map<GetDetailAccountQueryResponse>(account);
        return new BaseResponse<GetDetailAccountQueryResponse>(res, "Lấy thông tin nhân viên thành công");
    }
}