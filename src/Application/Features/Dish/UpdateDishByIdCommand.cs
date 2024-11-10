using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dish;

public class UpdateDishByIdCommandResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }
}

public class UpdateDishByIdCommand : IRequest<BaseResponse<UpdateDishByIdCommandResponse>>
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? Price { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Status { get; set; }
}

public class
    UpdateDishByIdCommandHandler : IRequestHandler<UpdateDishByIdCommand, BaseResponse<UpdateDishByIdCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateDishByIdCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<UpdateDishByIdCommandResponse>> Handle(UpdateDishByIdCommand request,
        CancellationToken cancellationToken)
    {
        var dish = await _context.Dishes.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        dish.Name = request.Name;   
        dish.Price = request.Price;
        dish.Description = request.Description;
        dish.Image = request.Image;
        dish.Status = request.Status;

        _context.Dishes.Update(dish);

        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<UpdateDishByIdCommandResponse>(dish);

        return new BaseResponse<UpdateDishByIdCommandResponse>(res, "Cập nhật món ăn thành công!");
    }
}