using Application.Common.Interfaces;
using Application.Features.Guest;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class GetOrdersByDateQueryResponse
{
    public int? Id { get; set; }
    public int? GuestId { get; set; }
    public int? TableNumber { get; set; }
    public int? DishSnapshotId { get; set; }
    public int? Quantity { get; set; }
    public int? OrderHandlerId { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public GuestInforResponse? Guest { get; set; }
    public DishSnapshotResponse? DishSnapshot { get; set; }
    public AccountResponse? OrderHandler { get; set; }
}

public class GetOrdersByDateQuery : IRequest<BaseResponse<List<GetOrdersByDateQueryResponse>>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class
    GetOrdersByDateQueryHandler : IRequestHandler<GetOrdersByDateQuery,
    BaseResponse<List<GetOrdersByDateQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetOrdersByDateQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GetOrdersByDateQueryResponse>>> Handle(GetOrdersByDateQuery request,
        CancellationToken cancellationToken)
    {
        var fromDate = request.FromDate;
        var toDate = request.ToDate;

        var test = _context.Orders.ToList();
        var query = _context.Orders
            .Include(i => i.Guest)
            .Include(i => i.DishSnapshot)
            .Include(i => i.OrderHandler)
            // .Include(i => i.OrderHandler)
            .AsQueryable();
        if (fromDate.HasValue) query = query.Where(o => o.CreatedAt >= fromDate);

        if (toDate.HasValue) query = query.Where(o => o.CreatedAt <= toDate);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        var ordersResponse = _mapper.Map<List<GetOrdersByDateQueryResponse>>(orders);

        return new BaseResponse<List<GetOrdersByDateQueryResponse>>(ordersResponse,
            "Lấy danh sách đơn hàng thành công");
    }
}