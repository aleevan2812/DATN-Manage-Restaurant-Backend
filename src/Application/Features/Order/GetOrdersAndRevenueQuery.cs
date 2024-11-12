using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class GetOrdersAndRevenueQueryResponse
{
    public string? Date { get; set; }
    public int? Order { get; set; }
    public int? Revenue { get; set; }
}

public class GetOrdersAndRevenueQuery : IRequest<BaseResponse<List<GetOrdersAndRevenueQueryResponse>>>
{
   
}

public class GetOrdersAndRevenueQueryHandler : IRequestHandler<GetOrdersAndRevenueQuery, BaseResponse<List<GetOrdersAndRevenueQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdersAndRevenueQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<BaseResponse<List<GetOrdersAndRevenueQueryResponse>>> Handle(GetOrdersAndRevenueQuery request, CancellationToken cancellationToken)
    {
        
        var orders = _context.Orders.Include(i => i.DishSnapshot).Where(i => i.Status == OrderStatus.Paid).ToList();
        var result = orders
            .GroupBy(o => o.CreatedAt?.Date) // Nhóm theo ngày
            .Select(g => new GetOrdersAndRevenueQueryResponse
            {
                Date = g.Key.HasValue ? g.Key.Value.ToString("yyyy-MM-dd") : null, // Chuyển định dạng ngày
                Order = g.Count(), // Đếm số lượng order
                Revenue = g.Sum(o => o.DishSnapshot?.Price ?? 0) // Tổng Revenue cho ngày đó
            }).ToList();
        
        return new BaseResponse<List<GetOrdersAndRevenueQueryResponse>>(result);
    }
}