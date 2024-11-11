using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Guest;

public class GetGuestOrderQueryResponse
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

public class GetGuestOrderQuery : IRequest<BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
}

public class
    GetGuestOrderQueryHandler : IRequestHandler<GetGuestOrderQuery, BaseResponse<List<GuestCreateOrderCommandResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public GetGuestOrderQueryHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GuestCreateOrderCommandResponse>>> Handle(GetGuestOrderQuery request,
        CancellationToken cancellationToken)
    {
        var guestId = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;
        var guest = await _context.Guests.FirstOrDefaultAsync(i => i.Id == int.Parse(guestId), cancellationToken);

        var guestDto = _mapper.Map<GuestInforResponse>(guest);
        var orders = _context.Orders.Where(i => i.GuestId == int.Parse(guestId)).ToList();
        var ordersDto = _mapper.Map<List<GuestCreateOrderCommandResponse>>(orders);

        foreach (var order in ordersDto)
        {
            var dishSnapshot =
                await _context.DishSnapshots.FirstOrDefaultAsync(i => i.Id == order.DishSnapshotId, cancellationToken);

            order.Guest = guestDto;
            order.DishSnapshot = _mapper.Map<DishSnapshotResponse>(dishSnapshot);
        }

        return new BaseResponse<List<GuestCreateOrderCommandResponse>>(ordersDto, "Lấy danh sách đơn hàng thành công");
    }
}