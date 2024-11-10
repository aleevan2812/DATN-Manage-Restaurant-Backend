using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using Application.Services;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using Core.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Guest;

public class LoginGuestCommandResponse
{
    public LoginGuestCommandResponse(GuestDto guest, string accessToken, string refreshToken)
    {
        Guest = guest;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public GuestDto Guest { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}

public class LoginGuestCommand : IRequest<BaseResponse<LoginGuestCommandResponse>>
{
    public string Name { get; set; }
    public string Token { get; set; }
    public int TableNumber { get; set; }
}

public class LoginGuestCommandHandler : IRequestHandler<LoginGuestCommand, BaseResponse<LoginGuestCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LoginGuestCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<LoginGuestCommandResponse>> Handle(LoginGuestCommand request,
        CancellationToken cancellationToken)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(
            x => x.Number == request.TableNumber && x.Token == request.Token, cancellationToken);

        if (table == null)
            throw new BadRequestException(null, "Bàn không tồn tại hoặc mã token không đúng",
                HttpStatusCode.BadRequest);
        if (table.Status == Status.Hidden)
            throw new BadRequestException(null, "Bàn này đã bị ẩn, hãy chọn bàn khác để đăng nhập",
                HttpStatusCode.BadRequest);

        if (table.Status == Status.Reserved)
            throw new BadRequestException(null, "Bàn đã được đặt trước, hãy liên hệ nhân viên để được hỗ trợ",
                HttpStatusCode.BadRequest);

        var guest = _mapper.Map<Core.Entities.Guest>(request);

        _context.Guests.Add(guest);
        await _context.SaveChangesAsync(cancellationToken);

        var refreshToken = TokenService.GenerateAccessToken(guest.Id, Role.Guest,
            "hoc_lap_trinh_edu_duthanhduoc_com_access", DateTime.UtcNow.AddHours(12));
        var accessToken = TokenService.GenerateAccessToken(guest.Id, Role.Guest,
            "hoc_lap_trinh_edu_duthanhduoc_com_access", DateTime.UtcNow.AddMinutes(15));

        guest.RefreshTokenExpiresAt = DateTime.UtcNow.AddMinutes(15);
        guest.RefreshToken = refreshToken;

        _context.Guests.Update(guest);
        await _context.SaveChangesAsync(cancellationToken);

        var guestDto = _mapper.Map<GuestDto>(guest);
        guestDto.Role = Role.Guest;
        var res = new LoginGuestCommandResponse(guestDto, accessToken, refreshToken);

        return new BaseResponse<LoginGuestCommandResponse>(res, "Đăng nhập thành công");
    }
}