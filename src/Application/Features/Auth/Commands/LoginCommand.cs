using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using Core.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands;

public class LoginResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public AccountDto? Account { get; set; }
}

public class LoginCommand : IRequest<BaseResponse<LoginResponse>>
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, BaseResponse<LoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IApplicationDbContext context, IMapper mapper, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<BaseResponse<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var account = await
            _context.Accounts.FirstOrDefaultAsync(a => a.Email == request.Email && a.Password == request.Password,
                cancellationToken);
        if (account == null)
            throw new EntityErrorException(new List<ValidationError>
            {
                new("Email && Password", "Email hoặc mật khẩu không đúng")
            }, "Lỗi xảy ra khi xác thực dữ liệu...", HttpStatusCode.UnprocessableEntity);

        var accountDto = _mapper.Map<AccountDto>(account);

        var jwtTokenId = $"JTI{Guid.NewGuid()}";
        var accessToken =
            _tokenService.GenerateAccessToken(account.Id, account.Role, jwtTokenId);
        var refreshToken =
            await _tokenService.GenerateRefreshToken(account.Id, jwtTokenId, account.Role);

        var response = new LoginResponse
        {
            Account = accountDto,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };

        return new BaseResponse<LoginResponse>(response, "Đăng nhập thành công");
    }
}