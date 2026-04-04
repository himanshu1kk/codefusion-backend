namespace CFFFusions.Services;

public interface IHandleVerificationService
{
    Task<string> StartVerificationAsync(string userId, string handle);
    Task<string> VerifyAsync(string userId, string otp);
}