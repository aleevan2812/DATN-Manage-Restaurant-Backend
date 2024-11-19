using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Guest;

public class GuestInforResponse
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? TableNumber { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DishSnapshotResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }
    public int DishId { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AccountResponse
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }
}

public class GuestCreateOrderCommandResponse
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

public class GuestCreateOrderRequest
{
    public int DishId { get; set; }
    public int Quantity { get; set; }
}

public class GuestCreateOrderCommand : IRequest<BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    public List<GuestCreateOrderRequest>? Orders { get; set; }
}

public class GuestCreateOrderCommandHandler : IRequestHandler<GuestCreateOrderCommand,
    BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public GuestCreateOrderCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper, INotificationService notificationService)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<BaseResponse<List<GuestCreateOrderCommandResponse>>> Handle(GuestCreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var guestId = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;
        var guest = await _context.Guests.FirstOrDefaultAsync(i => i.Id == int.Parse(guestId), cancellationToken);

        if (guest?.TableNumber == null)
            throw new BadRequestException(null,
                "Bàn của bạn đã bị xóa, vui lòng đăng xuất và đăng nhập lại một bàn mới",
                HttpStatusCode.BadRequest);

        var table = await _context.Tables.FirstOrDefaultAsync(i => i.Number == guest.TableNumber, cancellationToken);

        if (table?.Status == Status.Hidden)
            throw new BadRequestException(null,
                $"Bàn {table.Number} đã bị ẩn, vui lòng đăng xuất và chọn bàn khác",
                HttpStatusCode.BadRequest);
        if (table?.Status == Status.Reserved)
            throw new BadRequestException(null,
                $"Bàn {table.Number} đã bị ẩn, đã được đặt trước, vui lòng đăng xuất và chọn bàn khác",
                HttpStatusCode.BadRequest);

        var orders = new List<GuestCreateOrderCommandResponse>();
        var newDishSnapshots = new List<DishSnapshot>();
        var newOrders = new List<Core.Entities.Order>();
        await using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
        {
            try
            {
                if (request.Orders != null)
                {
                    foreach (var order in request.Orders)
                    {
                        var dish = await _context.Dishes.FirstOrDefaultAsync(i => i.Id == order.DishId,
                            cancellationToken);
                        if (dish == null) continue;
                        if (dish.Status == DishStatus.Unavailable)
                            throw new BadRequestException(null,
                                $"Món {dish.Name} đã hết",
                                HttpStatusCode.BadRequest);
                        if (dish.Status == DishStatus.Hidden)
                            throw new BadRequestException(null,
                                $"Món {dish.Name} không thể đặt",
                                HttpStatusCode.BadRequest);

                        var dishSnapshot = new DishSnapshot
                        {
                            Name = dish.Name,
                            Price = dish.Price,
                            Description = dish.Description,
                            Status = dish.Status,
                            Image = dish.Image,
                            DishId = dish.Id
                        };
                        newDishSnapshots.Add(dishSnapshot);
                    }

                    await _context.DishSnapshots.AddRangeAsync(newDishSnapshots, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);

                    foreach (var order in request.Orders)
                    {
                        var dishSnapshot = newDishSnapshots.FirstOrDefault(ds => ds.DishId == order.DishId);
                        if (dishSnapshot == null)
                            continue;
                        var createdOrder = new Core.Entities.Order
                        {
                            DishSnapshotId = dishSnapshot.Id,
                            GuestId = int.Parse(guestId),
                            Quantity = order.Quantity,
                            TableNumber = guest.TableNumber,
                            OrderHandlerId = null,
                            Status = OrderStatus.Pending
                        };
                        newOrders.Add(createdOrder);

                        var orderResponse = _mapper.Map<GuestCreateOrderCommandResponse>(createdOrder);
                        orderResponse.Guest = _mapper.Map<GuestInforResponse>(guest);
                        orderResponse.DishSnapshot = _mapper.Map<DishSnapshotResponse>(dishSnapshot);
                        orderResponse.OrderHandler = null;
                        orders.Add(orderResponse);
                    }

                    await _context.Orders.AddRangeAsync(newOrders, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        _ = _notificationService.SendMessage("new-order", orders);

        return new BaseResponse<List<GuestCreateOrderCommandResponse>>(orders, "Đặt món thành công");
    }
}