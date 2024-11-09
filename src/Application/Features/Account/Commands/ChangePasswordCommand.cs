using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public class ChangePasswordCommandResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Avatar { get; set; }
}

public class ChangePasswordCommand : IRequest<BaseResponse<ChangePasswordCommandResponse>>
{
    public string OldPassword { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class
    ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, BaseResponse<ChangePasswordCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public ChangePasswordCommandHandler(IApplicationDbContext context, IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseResponse<ChangePasswordCommandResponse>> Handle(ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;
        var account =
            await _context.Accounts.FirstOrDefaultAsync(i => i.Id == int.Parse(userIdClaim), cancellationToken);

        if (account.Password != request.OldPassword)
            throw new EntityErrorException(new List<ValidationError>
            {
                new("oldPassword", "Mật khẩu cũ không đúng")
            }, "Lỗi xảy ra khi xác thực dữ liệu...", HttpStatusCode.UnprocessableEntity);

        account.Password = request.Password;

        _context.Accounts.Update(account);

        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<ChangePasswordCommandResponse>(account);

        return new BaseResponse<ChangePasswordCommandResponse>(res, "Đổi mật khẩu thành công");
    }
}