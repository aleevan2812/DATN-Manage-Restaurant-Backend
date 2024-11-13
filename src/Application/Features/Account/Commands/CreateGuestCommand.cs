using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using Core.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public class CreateGuestCommand : IRequest<BaseResponse<GuestDto>>
{
    public string? Name { get; set; }
    public int? TableNumber { get; set; }
}

public class CreateGuestCommandHandler : IRequestHandler<CreateGuestCommand, BaseResponse<GuestDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateGuestCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GuestDto>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
    {
        var table = await _context.Tables.FirstOrDefaultAsync(t => t.Number == request.TableNumber, cancellationToken);

        if (table == null)
            throw new BadRequestException(null,
                "Bàn không tồn tại",
                HttpStatusCode.BadRequest);

        if (table?.Status == Status.Hidden)
            throw new BadRequestException(null,
                $"Bàn {table.Number} đã bị ẩn, vui lòng chọn bàn khác",
                HttpStatusCode.BadRequest);
        var guest = new Core.Entities.Guest
        {
            Name = request.Name,
            TableNumber = request.TableNumber
        };

        _context.Guests.Add(guest);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<GuestDto>(guest);
        return new BaseResponse<GuestDto>(res, "Tạo tài khoản khách thành công");
    }
}