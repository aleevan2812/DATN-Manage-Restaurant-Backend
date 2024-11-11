using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dish;

public class GetDishByIdQueryResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GetDishByIdQuery : IRequest<BaseResponse<GetDishByIdQueryResponse>>
{
    public int Id { get; set; }
}

public class GetDishByIdQueryHandler : IRequestHandler<GetDishByIdQuery, BaseResponse<GetDishByIdQueryResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDishByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetDishByIdQueryResponse>> Handle(GetDishByIdQuery request,
        CancellationToken cancellationToken)
    {
        var dish = await _context.Dishes.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        var res = _mapper.Map<GetDishByIdQueryResponse>(dish);

        return new BaseResponse<GetDishByIdQueryResponse>(res, "Lấy thông tin món ăn thành công!");
    }
}