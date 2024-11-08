using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public class UpdateMeCommandResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }
}

public class UpdateMeCommand : IRequest<BaseResponse<UpdateMeCommandResponse>>
{
    public UpdateMeCommand(int? userId, string? name = null, string? avatar = null)
    {
        UserId = userId;
        Name = name;
        Avatar = avatar;
    }

    public int? UserId { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
}

public class UpdateMeCommandHandler : IRequestHandler<UpdateMeCommand, BaseResponse<UpdateMeCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdateMeCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<UpdateMeCommandResponse>> Handle(UpdateMeCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == request.UserId,
            cancellationToken);

        if (request.Avatar != null)
            account.Avatar = request.Avatar;
        account.Name = request.Name;

        _context.Accounts.Update(account);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<UpdateMeCommandResponse>(account);

        return new BaseResponse<UpdateMeCommandResponse>(res, "Cập nhật thông tin thành công");
    }
}