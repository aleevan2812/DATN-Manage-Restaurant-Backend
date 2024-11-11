using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public class DeleteEmployeeCommandResponse
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }
    public int? OwnerId { get; set; }
    public int Id { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class DeleteEmployeeCommand : IRequest<BaseResponse<DeleteEmployeeCommandResponse>>
{
    public DeleteEmployeeCommand(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}

public class
    DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, BaseResponse<DeleteEmployeeCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public DeleteEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<DeleteEmployeeCommandResponse>> Handle(DeleteEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        var res = _mapper.Map<DeleteEmployeeCommandResponse>(account);

        if (account != null) _context.Accounts.Remove(account);
        await _context.SaveChangesAsync(cancellationToken);

        return new BaseResponse<DeleteEmployeeCommandResponse>(res, "Xóa thành công");
    }
}