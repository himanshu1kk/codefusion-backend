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
    public static readonly int INVALID_EMAIL = 4010;
public static readonly int EMAIL_ALREADY_VERIFIED = 4011;
public static readonly int OTP_EXPIRED = 4012;
public static readonly int OTP_ATTEMPTS_EXCEEDED = 4013;
public static readonly int OTP_NOT_FOUND = 4014;
public static readonly int OTP_INVALID = 4015;

    public static readonly Dictionary<int, IActionResult> ErrorMappings = new()
    {
        [INTERNAL_ERROR] = new ObjectResult("Internal Server Error") { StatusCode = 500 },
        [BAD_REQUEST] = new ObjectResult("Bad Request") { StatusCode = 400 },
        [USER_NOT_FOUND] = new ObjectResult("User not found") { StatusCode = 404 },
        [CODEFORCES_API_FAILED] = new ObjectResult("Codeforces API failed") { StatusCode = 502 },
        [INVALID_EMAIL] = new ObjectResult("Invalid email format") { StatusCode = 400 },
[EMAIL_ALREADY_VERIFIED] = new ObjectResult("Email already registered and verified") { StatusCode = 409 },
[OTP_EXPIRED] = new ObjectResult("OTP expired") { StatusCode = 410 },
[OTP_ATTEMPTS_EXCEEDED] = new ObjectResult("OTP attempts exceeded") { StatusCode = 429 },
 [OTP_NOT_FOUND] = new ObjectResult("OTP expired or not found") { StatusCode = 404 },
    [OTP_INVALID] = new ObjectResult("Invalid OTP") { StatusCode = 400 },




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
