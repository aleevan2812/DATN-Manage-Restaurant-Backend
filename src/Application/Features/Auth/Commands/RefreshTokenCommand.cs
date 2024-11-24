using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using Core.Dtos;
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
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IMapper mapper, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
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

        var accessToken = _tokenService.GenerateAccessToken(account.Id, account.Role, DateTime.UtcNow.AddHours(24));
        var refreshToken =
            await _tokenService.GenerateRefreshToken(account.Id, account.Role, DateTime.UtcNow.AddHours(24));

        var response = new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new BaseResponse<RefreshTokenResponse>(response, "Lấy token mới thành công");
    }
}