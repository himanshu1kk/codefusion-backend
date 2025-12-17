namespace Cff.Models;

public class BaseResponse
{
    public int ApiResponseCode { get; set; }
    public string Message { get; set; }

    public BaseResponse(int code, string message)
    {
        ApiResponseCode = code;
        Message = message;
    }
}
