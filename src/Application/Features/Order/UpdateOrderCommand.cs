using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Features.Guest;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class UpdateOrderCommand : IRequest<BaseResponse<GuestCreateOrderCommandResponse>>
{
    public int? OrderId { get; set; }
    public string? Status { get; set; }
    public int? DishId { get; set; }
    public int? Quantity { get; set; }
}

public class
    UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, BaseResponse<GuestCreateOrderCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public UpdateOrderCommandHandler(IApplicationDbContext context, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _notificationService = notificationService;
    }

    public async Task<BaseResponse<GuestCreateOrderCommandResponse>> Handle(UpdateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(i => i.OrderHandler)
            .Include(i => i.Guest)
            .Include(i => i.DishSnapshot)
            .FirstOrDefaultAsync(i => i.Id == request.OrderId, cancellationToken);

        if (IsWithinLast24Hours((DateTime)order.CreatedAt) == false)
            throw new BadRequestException(null, "Quá hạn 24h chỉnh sửa đơn hàng!",
                HttpStatusCode.BadRequest);

        if (order != null)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                order.Quantity = request.Quantity;
                order.Status = request.Status;

                var userId = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;
                if (userId != null)
                    order.OrderHandlerId = int.Parse(userId);

                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                var res = _mapper.Map<GuestCreateOrderCommandResponse>(order);
                _ = _notificationService.SendMessage("update-order", res);

                return new BaseResponse<GuestCreateOrderCommandResponse>(res
                    , "Cập nhật đơn hàng thành công");
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        return new BaseResponse<GuestCreateOrderCommandResponse>();
    }

    public bool IsWithinLast24Hours(DateTime createdAt)
    {
        if (createdAt == null) return false;
        // Get the current time
        var now = DateTime.Now;

        // Calculate the time difference
        var timeDifference = now - createdAt;

        // Check if the difference is within 24 hours
        return timeDifference.TotalHours <= 24;
    }
}