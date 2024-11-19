using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Features.Guest;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Order;

public class CreateOrderCommand : IRequest<BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    public int? GuestId { get; set; }
    public List<GuestCreateOrderRequest>? Orders { get; set; }
}

public class
    CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public CreateOrderCommandHandler(IApplicationDbContext context, IMapper mapper,
        INotificationService notificationService)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<BaseResponse<List<GuestCreateOrderCommandResponse>>> Handle(CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var guest = await _context.Guests.FirstOrDefaultAsync(i => i.Id == request.GuestId, cancellationToken);

        var table = await _context.Tables.FirstOrDefaultAsync(i => i.Number == guest.TableNumber, cancellationToken);
        if (table == null)
            throw new BadRequestException(null,
                "Bàn của bạn đã bị xóa, vui lòng đăng xuất và đăng nhập lại một bàn mới",
                HttpStatusCode.BadRequest);

        if (table.Status == Status.Hidden)
            throw new BadRequestException(null,
                $"Bàn {table.Number} đã bị ẩn, vui lòng đăng xuất và chọn bàn khác",
                HttpStatusCode.BadRequest);
        if (table.Status == Status.Reserved)
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
                            GuestId = request.GuestId,
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

                _ = _notificationService.SendMessage("new-order", orders);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }


        return new BaseResponse<List<GuestCreateOrderCommandResponse>>(orders, "Đặt món thành công");
    }
}