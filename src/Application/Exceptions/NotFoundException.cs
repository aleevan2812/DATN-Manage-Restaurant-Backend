using System.Net;

namespace Application.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string? message) : base(message)
    {
        Message = message;
    }

    public HttpStatusCode StatusCode { get; } = HttpStatusCode.NotFound;
    public string? Message { get; set; } = "Lỗi xảy ra khi xác thực dữ liệu...";
}