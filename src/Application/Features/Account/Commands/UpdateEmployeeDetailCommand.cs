using System.Net;
using Application.Common.Interfaces;
using Application.Exceptions;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Account.Commands;

public class UpdateEmployeeDetailCommandResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? Avatar { get; set; }
}

public class UpdateEmployeeDetailCommand : IRequest<BaseResponse<UpdateEmployeeDetailCommandResponse>>
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
    public bool? ChangePassword { get; set; }
    public string? Avatar { get; set; }
    public int? Id { get; set; }
}

public class UpdateEmployeeDetailCommandHandler : IRequestHandler<UpdateEmployeeDetailCommand,
    BaseResponse<UpdateEmployeeDetailCommandResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;

    public UpdateEmployeeDetailCommandHandler(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
    }

    public async Task<BaseResponse<UpdateEmployeeDetailCommandResponse>> Handle(UpdateEmployeeDetailCommand request,
        CancellationToken cancellationToken)
    {
        if (await _context.Accounts.AnyAsync(x => x.Email == request.Email && x.Id != request.Id))
            throw new EntityErrorException(new List<ValidationError>
            {
                new("email", "Email đã tồn tại")
            }, "Lỗi xảy ra khi xác thực dữ liệu...", HttpStatusCode.UnprocessableEntity);

        var emp = await _context.Accounts.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (emp == null) throw new NotFoundException("Không tìm thấy nhân viên");


        emp.Email = request.Email;
        emp.Name = request.Name;
        if (request.ChangePassword == true) emp.Password = request.Password;
        if (request.Avatar != null) emp.Avatar = request.Avatar;
        _context.Accounts.Update(emp);

        await _context.SaveChangesAsync(cancellationToken);

        var res = _mapper.Map<UpdateEmployeeDetailCommandResponse>(emp);

        return new BaseResponse<UpdateEmployeeDetailCommandResponse>(res, "Cập nhật thành công");
    }
}