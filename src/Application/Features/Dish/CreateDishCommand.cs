using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;

namespace Application.Features.Dish;

public class CreateDishCommandResponse
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

public class CreateDishCommand : IRequest<BaseResponse<CreateDishCommandResponse>>
{
    public string? Name { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }
}

public class CreateDishCommandHandler : IRequestHandler<CreateDishCommand, BaseResponse<CreateDishCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateDishCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<CreateDishCommandResponse>> Handle(CreateDishCommand request,
        CancellationToken cancellationToken)
    {
        var dish = _mapper.Map<Core.Entities.Dish>(request);

        _context.Dishes.Add(dish);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<CreateDishCommandResponse>(dish);

        return new BaseResponse<CreateDishCommandResponse>(res, "Tạo món ăn thành công!");
    }
}