using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;

namespace Application.Features.Table.Queries;

public class GetTablesQueryResponse
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetTablesQuery : IRequest<BaseResponse<List<GetTablesQueryResponse>>>
{
}

public class GetTablesQueryHandler : IRequestHandler<GetTablesQuery, BaseResponse<List<GetTablesQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTablesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GetTablesQueryResponse>>> Handle(GetTablesQuery request,
        CancellationToken cancellationToken)
    {
        var tables = _context.Tables.ToList();
        var res = _mapper.Map<List<GetTablesQueryResponse>>(tables);

        if (tables.Any())
            return new BaseResponse<List<GetTablesQueryResponse>>(res, "Lấy danh sách bàn thành công!");

        return new BaseResponse<List<GetTablesQueryResponse>>(new List<GetTablesQueryResponse>(),
            "Lấy danh sách bàn thành công!");
    }
}