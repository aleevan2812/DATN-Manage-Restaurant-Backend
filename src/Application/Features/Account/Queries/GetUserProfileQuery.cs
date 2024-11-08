using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Queries;

public class GetUserProfileResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }
}

public class GetUserProfileQuery : IRequest<BaseResponse<GetUserProfileResponse>>
{
    public GetUserProfileQuery(int userId)
    {
        UserId = userId;
    }

    public int UserId { get; set; }
}

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, BaseResponse<GetUserProfileResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserProfileQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BaseResponse<GetUserProfileResponse>> Handle(GetUserProfileQuery request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(x => x.Id == request.UserId);

        var res = _mapper.Map<GetUserProfileResponse>(account);
        return new BaseResponse<GetUserProfileResponse>(res, "Lấy thông tin thành công");
    }
}