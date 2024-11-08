using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using Core.Const;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public class CreateEmployeeCommandResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? Role { get; set; }
}

public class CreateEmployeeCommand : IRequest<BaseResponse<CreateEmployeeCommandResponse>>
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public string? Avatar { get; set; }
}

public class
    CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, BaseResponse<CreateEmployeeCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public CreateEmployeeCommandHandler(IApplicationDbContext context, IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<BaseResponse<CreateEmployeeCommandResponse>> Handle(CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FirstOrDefaultAsync(i => i.Email == request.Email);

        if (account != null)
            throw new EntityErrorException(new List<ValidationError>
            {
                new("email", "Email đã tồn tại")
            }, "Lỗi xảy ra khi xác thực dữ liệu...", HttpStatusCode.UnprocessableEntity);

        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId").Value;

        account = _mapper.Map<Core.Entities.Account>(request);
        account.OwnerId = int.Parse(userIdClaim);
        account.Role = Role.Employee;

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<CreateEmployeeCommandResponse>(account);

        return new BaseResponse<CreateEmployeeCommandResponse>(res, "Tạo tài khoản thành công");
    }
}