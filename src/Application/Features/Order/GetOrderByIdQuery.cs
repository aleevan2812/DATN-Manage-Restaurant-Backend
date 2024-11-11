using Application.Common.Interfaces;
using Application.Features.Guest;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class TableResponse
{
    public int? Number { get; set; }
    public int? Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetOrderByIdQueryResponse
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
    public TableResponse? Table { get; set; }
}

public class GetOrderByIdQuery : IRequest<BaseResponse<GetOrderByIdQueryResponse>>
{
    public int? Id { get; set; }
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, BaseResponse<GetOrderByIdQueryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetOrderByIdQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetOrderByIdQueryResponse>> Handle(GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(o => o.OrderHandler)
            .Include(i => i.DishSnapshot)
            .Include(i => i.Guest)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
        
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Id == order.TableNumber, cancellationToken);
        
        var res = _mapper.Map<GetOrderByIdQueryResponse>(order);
        res.Table = _mapper.Map<TableResponse>(table);

        return new BaseResponse<GetOrderByIdQueryResponse>(res, "Lấy đơn hàng thành công");
    }
}