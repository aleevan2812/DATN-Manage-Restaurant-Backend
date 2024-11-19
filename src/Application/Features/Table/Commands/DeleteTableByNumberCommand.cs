using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Table.Commands;

public class DeleteTableByNumberCommandResponse
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DeleteTableByNumberCommand : IRequest<BaseResponse<DeleteTableByNumberCommandResponse>>
{
    public DeleteTableByNumberCommand(int n)
    {
        Number = n;
    }

    public int? Number { get; set; }
}

public class DeleteTableByNumberCommandHandler : IRequestHandler<DeleteTableByNumberCommand,
    BaseResponse<DeleteTableByNumberCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteTableByNumberCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<DeleteTableByNumberCommandResponse>> Handle(DeleteTableByNumberCommand request,
        CancellationToken cancellationToken)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(i => i.Number == request.Number, cancellationToken);

        var res = _mapper.Map<DeleteTableByNumberCommandResponse>(table);

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync(cancellationToken);

        return new BaseResponse<DeleteTableByNumberCommandResponse>(res, "Xóa bàn thành công!");
    }
}