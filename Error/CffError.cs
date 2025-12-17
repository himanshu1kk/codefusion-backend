using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Cff.Models;

namespace Cff.Error.Exceptions;

public class CffError : Exception
{
    // Error Codes
    public static readonly int INTERNAL_ERROR = 5000;
    public static readonly int BAD_REQUEST = 4000;
    public static readonly int USER_NOT_FOUND = 4004;
    public static readonly int CODEFORCES_API_FAILED = 4008;

    public static readonly Dictionary<int, IActionResult> ErrorMappings = new()
    {
        [INTERNAL_ERROR] = new ObjectResult("Internal Server Error") { StatusCode = 500 },
        [BAD_REQUEST] = new ObjectResult("Bad Request") { StatusCode = 400 },
        [USER_NOT_FOUND] = new ObjectResult("User not found") { StatusCode = 404 },
        [CODEFORCES_API_FAILED] = new ObjectResult("Codeforces API failed") { StatusCode = 502 },
    };

    public BaseResponse ErrorResponse { get; set; }

    public CffError(BaseResponse errorResponse, string message = "", Exception ex = null)
        : base($"{message} && {ex?.Message}")
    {
        ErrorResponse = errorResponse;
    }

    public override string ToString()
    {
        return $"{base.ToString()} <<< {JsonSerializer.Serialize(ErrorResponse)} >>>";
    }

    public static void AssertOrThrow(bool condition, int errorCode, string message)
    {
        if (!condition)
            throw new CffError(new BaseResponse(errorCode, message));
    }
}
