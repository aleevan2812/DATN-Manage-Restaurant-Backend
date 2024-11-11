using Application.Common.Interfaces;
using Application.Features.Guest;
using Application.Services;
using AutoMapper;
using Common.Models.Response;
using Core.Entities;
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
    private readonly ISignalRService _signalRService;

    public UpdateOrderCommandHandler(IApplicationDbContext context, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, ISignalRService signalRService)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _signalRService = signalRService;
    }

    public async Task<BaseResponse<GuestCreateOrderCommandResponse>> Handle(UpdateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .Include(i => i.OrderHandler)
            .Include(i => i.Guest)
            .FirstOrDefaultAsync(i => i.Id == request.OrderId, cancellationToken);
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

                var dish = await _context.Dishes.FirstOrDefaultAsync(i => i.Id == request.DishId, cancellationToken);
                var dishSnapshot = new DishSnapshot
                {
                    Name = dish.Name,
                    Price = dish.Price,
                    Description = dish.Description,
                    Status = dish.Status,
                    Image = dish.Image,
                    DishId = dish.Id
                };
                // create DishSnapshot
                await _context.DishSnapshots.AddAsync(dishSnapshot, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                order.DishSnapshotId = dishSnapshot.Id;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                var res = _mapper.Map<GuestCreateOrderCommandResponse>(order);
                _ = _signalRService.SendMessage("update-order", res);

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
}