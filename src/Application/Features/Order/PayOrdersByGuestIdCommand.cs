using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Features.Guest;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class PayOrdersByGuestIdCommand : IRequest<BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    public int? GuestId { get; set; }
}

public class PayOrdersByGuestIdCommandHandler : IRequestHandler<PayOrdersByGuestIdCommand,
    BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public PayOrdersByGuestIdCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GuestCreateOrderCommandResponse>>> Handle(PayOrdersByGuestIdCommand request,
        CancellationToken cancellationToken)
    {
        var orders = _context.Orders
            .Include(i => i.OrderHandler)
            .Include(i => i.Guest)
            .Include(i => i.DishSnapshot)
            .Where(i => i.GuestId == request.GuestId).ToList();
        
        if (!orders.Any())  throw new BadRequestException(null, "Không có hóa đơn nào cần thanh toán",
            HttpStatusCode.BadRequest);

        foreach (var order in orders)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;
            if (userId != null) order.OrderHandlerId = int.Parse(userId);
            order.Status = OrderStatus.Paid;
        }

        _context.Orders.UpdateRange(orders);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<List<GuestCreateOrderCommandResponse>>(orders);

        return new BaseResponse<List<GuestCreateOrderCommandResponse>>(
            _mapper.Map<List<GuestCreateOrderCommandResponse>>(orders), $"Thanh toán thành công {orders.Count} đơn");
    }
}