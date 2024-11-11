using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries;

public class GetGuestsByDateQueryResponse
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? TableNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetGuestsByDateQuery : IRequest<BaseResponse<List<GetGuestsByDateQueryResponse>>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class
    GetGuestsByDateQueryHandler : IRequestHandler<GetGuestsByDateQuery,
    BaseResponse<List<GetGuestsByDateQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetGuestsByDateQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GetGuestsByDateQueryResponse>>> Handle(GetGuestsByDateQuery request,
        CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate;
        var toDate = request.ToDate;

        var query = _context.Guests
            .AsQueryable();
        if (fromDate.HasValue) query = query.Where(o => o.CreatedAt >= fromDate);

        if (toDate.HasValue) query = query.Where(o => o.CreatedAt <= toDate);

        var guest = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        var res = _mapper.Map<List<GetGuestsByDateQueryResponse>>(guest);

        return new BaseResponse<List<GetGuestsByDateQueryResponse>>(res, "Lấy danh sách khách thành công");
    }
}