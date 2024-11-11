using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Table.Queries;

public class GetTableByNumberQueryResponse
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetTableByNumberQuery : IRequest<BaseResponse<GetTableByNumberQueryResponse>>
{
    public GetTableByNumberQuery(int number)
    {
        Number = number;
    }

    public int Number { get; set; }
}

public class
    GetTableByNumberQueryHandler : IRequestHandler<GetTableByNumberQuery, BaseResponse<GetTableByNumberQueryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTableByNumberQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetTableByNumberQueryResponse>> Handle(GetTableByNumberQuery request,
        CancellationToken cancellationToken)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Number == request.Number);

        var res = _mapper.Map<GetTableByNumberQueryResponse>(table);

        return new BaseResponse<GetTableByNumberQueryResponse>(res, "Lấy thông tin bàn thành công!");
    }
}