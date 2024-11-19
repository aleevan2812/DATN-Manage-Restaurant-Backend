using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using AutoMapper;
using Common.Models.Response;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Media.Commands;

public class UploadCommand : IRequest<BaseResponse<string>>
{
    public UploadCommand(IFormFile? file)
    {
        File = file;
    }

    public IFormFile? File { get; set; }
}

public class UploadCommandHandler : IRequestHandler<UploadCommand, BaseResponse<string>>
{
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UploadCommandHandler(IApplicationDbContext context, IMapper mapper, ICloudinaryService cloudinaryService)
    {
        _context = context;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<BaseResponse<string>> Handle(UploadCommand request, CancellationToken cancellationToken)
    {
        var imgUrl = await _cloudinaryService.UploadImageAsync(request.File);
        return new BaseResponse<string>(imgUrl, "Upload ảnh thành công");
    }
}