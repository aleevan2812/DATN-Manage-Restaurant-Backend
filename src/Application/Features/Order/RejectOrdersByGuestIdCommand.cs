using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Features.Guest;
using Application.Services;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class RejectOrdersByGuestIdCommand : IRequest<BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    public int? GuestId { get; set; }
}

public class RejectOrdersByGuestIdCommandHandler : IRequestHandler<RejectOrdersByGuestIdCommand,
    BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ISignalRService _signalRService;

    public RejectOrdersByGuestIdCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper, ISignalRService signalRService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _signalRService = signalRService;
    }

    public async Task<BaseResponse<List<GuestCreateOrderCommandResponse>>> Handle(RejectOrdersByGuestIdCommand request,
        CancellationToken cancellationToken)
    {
        var orders = _context.Orders
            .Include(i => i.OrderHandler)
            .Include(i => i.Guest)
            .Include(i => i.DishSnapshot)
            .Where(i => i.GuestId == request.GuestId).ToList();

        if (!orders.Any())
            throw new BadRequestException(null, "Không có hóa đơn nào cần từ chối",
                HttpStatusCode.BadRequest);

        foreach (var order in orders)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;
            if (userId != null) order.OrderHandlerId = int.Parse(userId);
            order.Status = OrderStatus.Rejected;
        }

        _context.Orders.UpdateRange(orders);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<List<GuestCreateOrderCommandResponse>>(orders);

        _ = _signalRService.SendMessage("reject", res);

        return new BaseResponse<List<GuestCreateOrderCommandResponse>>(
            res, $"Từ chối thành công {orders.Count} đơn");
    }
}