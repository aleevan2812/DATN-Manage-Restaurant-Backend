using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Table.Commands;

public class CreateTableCommandResponse
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTableCommand : IRequest<BaseResponse<CreateTableCommandResponse>>
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
}

public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, BaseResponse<CreateTableCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateTableCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<CreateTableCommandResponse>> Handle(CreateTableCommand request,
        CancellationToken cancellationToken)
    {
        if (await _context.Tables.AnyAsync(i => i.Number == request.Number, cancellationToken))
            throw new EntityErrorException(new List<ValidationError>
            {
                new("number", "Số bàn này đã tồn tại")
            }, "Lỗi xảy ra khi xác thực dữ liệu...", HttpStatusCode.UnprocessableEntity);

        var table = _mapper.Map<Core.Entities.Table>(request);
        table.Token = Guid.NewGuid().ToString("N");

        _context.Tables.Add(table);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<CreateTableCommandResponse>(table);
        return new BaseResponse<CreateTableCommandResponse>(res, "Tạo bàn thành công!");
    }
}