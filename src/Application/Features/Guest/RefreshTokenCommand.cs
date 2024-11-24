using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Guest;

public class RefreshTokenCommandResponse
{
    public string? RefreshToken { get; set; }
    public string? AccessToken { get; set; }
}

public class RefreshTokenCommand : IRequest<BaseResponse<RefreshTokenCommandResponse>>
{
    public string? RefreshToken { get; set; }
}

public class
    ResetPasswordCommandHandler : IRequestHandler<RefreshTokenCommand, BaseResponse<RefreshTokenCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;

    public ResetPasswordCommandHandler(IApplicationDbContext context, IMapper mapper, ITokenService tokenService)
    {
        _context = context;
        _mapper = mapper;
        _tokenService = tokenService;
    }

    public async Task<BaseResponse<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var principal =
            _tokenService.ValidateTokenAndGetClaims(request.RefreshToken);

        var userId = principal.FindFirst("userId")?.Value;
        var expClaim = principal.FindFirst("exp")?.Value;
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

        var jwtTokenId = $"JTI{Guid.NewGuid()}";
        var accessToken =
            _tokenService.GenerateAccessToken(int.Parse(userId), Role.Guest, jwtTokenId);
        var refreshToken =
            await _tokenService.GenerateRefreshToken(int.Parse(userId), jwtTokenId, Role.Guest, expiresAt);

        var guest = await _context.Guests.FirstOrDefaultAsync(i => i.Id == int.Parse(userId), cancellationToken);
        guest.RefreshToken = refreshToken;

        _context.Guests.Update(guest);
        await _context.SaveChangesAsync(cancellationToken);

        var res = new RefreshTokenCommandResponse { RefreshToken = refreshToken, AccessToken = accessToken };

        return new BaseResponse<RefreshTokenCommandResponse>(res, "Lấy token mới thành công");
    }
}