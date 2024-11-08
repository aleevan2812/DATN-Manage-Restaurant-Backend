namespace Common.Models.Response;

public class BaseResponse<T>
{
    public BaseResponse()
    {
    }

    public BaseResponse(T? data, string message = "")
    {
        Data = data;
        Message = message;
    }

    public T? Data { get; set; }
    public string? Message { get; set; }
}