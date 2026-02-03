using CFFFusions.Models;

namespace CFFFusions.Services;

public interface IRegistrationService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<string> VerifyOtpAsync(VerifyOtpRequest request);
}
