using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Table.Commands;

public class UpdateTableCommandResponse
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UpdateTableCommand : IRequest<BaseResponse<UpdateTableCommandResponse>>
{
    public int? Number { get; set; }
    public int Capacity { get; set; }
    public bool ChangeToken { get; set; }
    public string? Status { get; set; }
}

public class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, BaseResponse<UpdateTableCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateTableCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<UpdateTableCommandResponse>> Handle(UpdateTableCommand request,
        CancellationToken cancellationToken)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var table = await _context.Tables.FirstOrDefaultAsync(t => t.Number == request.Number, cancellationToken);
            if (table != null)
            {
                table.Capacity = request.Capacity;
                table.Status = request.Status;
                if (request.ChangeToken)
                {
                    table.Token = Guid.NewGuid().ToString("N");
                    var guests = _context.Guests
                        .Where(i => i.TableNumber == table.Number)
                        .ToList();
                    foreach (var guest in guests) guest.TableNumber = null;
                }

                _context.Tables.Update(table);
                await _context.SaveChangesAsync(cancellationToken);
                
                await transaction.CommitAsync(cancellationToken);

                var response = _mapper.Map<UpdateTableCommandResponse>(table);
                return new BaseResponse<UpdateTableCommandResponse>(response, "Cập nhật bàn thành công!");
            }

            return new BaseResponse<UpdateTableCommandResponse>(null, "Không tìm thấy bàn!");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}