using Microsoft.AspNetCore.Mvc;
using Cff.Error.Exceptions;

namespace Cff.Error.Extensions;

public static class CffErrorExtensions
{
    public static IActionResult ToActionResult(this CffError error)
    {
        if (
            CffError.ErrorMappings.TryGetValue(
                error.ErrorResponse.ApiResponseCode,
                out IActionResult value
            )
        )
        {
            var obj = value as ObjectResult;
            return new ObjectResult(error.ErrorResponse)
            {
                StatusCode = obj?.StatusCode ?? 500
            };
        }

        return new ObjectResult("Unknown Error") { StatusCode = 500 };
    }
}
