using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;

namespace Application.Features.Dish;

public class GetDishesQueryResponse
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

public class GetDishesQuery : IRequest<BaseResponse<List<GetDishesQueryResponse>>>
{
}

public class GetDishesQueryHandler : IRequestHandler<GetDishesQuery, BaseResponse<List<GetDishesQueryResponse>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDishesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<List<GetDishesQueryResponse>>> Handle(GetDishesQuery request,
        CancellationToken cancellationToken)
    {
        var dishes = _context.Dishes.OrderByDescending(x => x.CreatedAt).ToList();

        var res = _mapper.Map<List<GetDishesQueryResponse>>(dishes);

        return new BaseResponse<List<GetDishesQueryResponse>>(res, "Lấy danh sách món ăn thành công!");
    }
}