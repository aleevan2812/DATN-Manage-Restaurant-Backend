using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dish;

public class DeleteDishByIdCommandResponse
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

public class DeleteDishByIdCommand : IRequest<BaseResponse<DeleteDishByIdCommandResponse>>
{
    public int Id { get; set; }
}

public class
    DeleteDishByIdCommandHandler : IRequestHandler<DeleteDishByIdCommand, BaseResponse<DeleteDishByIdCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteDishByIdCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<DeleteDishByIdCommandResponse>> Handle(DeleteDishByIdCommand request,
        CancellationToken cancellationToken)
    {
        var dish = await _context.Dishes.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        var res = _mapper.Map<DeleteDishByIdCommandResponse>(dish);

        if (dish != null) _context.Dishes.Remove(dish);
        await _context.SaveChangesAsync(cancellationToken);

        return new BaseResponse<DeleteDishByIdCommandResponse>(res, "Xóa món ăn thành công!");
    }
}