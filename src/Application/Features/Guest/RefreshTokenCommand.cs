using System.Security.Claims;
using Application.Common.Interfaces;
using Application.Services;
using AutoMapper;
using Common.Models.Response;
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

    public ResetPasswordCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<RefreshTokenCommandResponse>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var principal =
            TokenService.ValidateTokenAndGetClaims(request.RefreshToken, "hoc_lap_trinh_edu_duthanhduoc_com_refresh");

        var userId = principal.FindFirst("userId")?.Value;
        var role = principal.FindFirst(ClaimTypes.Role)?.Value;
        var expClaim = principal.FindFirst("exp")?.Value;
        var expiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;

        var accessToken = TokenService.GenerateAccessToken(int.Parse(userId), role,
            "hoc_lap_trinh_edu_duthanhduoc_com_access", DateTime.Now.AddMinutes(15));
        var refreshToken = TokenService.GenerateRefreshToken(int.Parse(userId), role,
            "hoc_lap_trinh_edu_duthanhduoc_com_refresh", expiresAt);

        var guest = await _context.Guests.FirstOrDefaultAsync(i => i.Id == int.Parse(userId), cancellationToken);
        guest.RefreshToken = refreshToken;

        _context.Guests.Update(guest);
        await _context.SaveChangesAsync(cancellationToken);

        var res = new RefreshTokenCommandResponse { RefreshToken = refreshToken, AccessToken = accessToken };

        return new BaseResponse<RefreshTokenCommandResponse>(res, "Lấy token mới thành công");
    }
}