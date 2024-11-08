using System.Net;
using Application.Common.Interfaces;
using Application.Dtos;
using Application.Exceptions;
using Application.Services;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands;

public class RefreshTokenResponse
{
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
}

public class RefreshTokenCommand : IRequest<BaseResponse<RefreshTokenResponse>>
{
    public string? RefreshToken { get; set; }
}

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<RefreshTokenResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var account = await
            _context.Accounts.FirstOrDefaultAsync(a => a.Id == 1,
                cancellationToken);
        if (account == null)
            throw new EntityErrorException(new List<ValidationError>
            {
                new("Email && Password", "Email hoặc mật khẩu không đúng")
            }, "Lỗi xảy ra khi xác thực dữ liệu...", HttpStatusCode.UnprocessableEntity);

        var accountDto = _mapper.Map<AccountDto>(account);

        var accessToken = TokenService.GenerateAccessToken(account.Id, account.Role);
        var refreshToken = TokenService.GenerateAccessToken(account.Id, account.Role);

        var response = new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new BaseResponse<RefreshTokenResponse>(response, "Lấy token mới thành công");
    }
}