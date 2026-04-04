using Microsoft.Extensions.Caching.Memory;
using Cff.Error.Exceptions;
using Cff.Models;
using CFFFusions.Models;

namespace CFFFusions.Services;

public class HandleVerificationService : IHandleVerificationService
{
    private readonly ICodeforcesClient _cf;
    private readonly IMemoryCache _cache;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;

    private static readonly TimeSpan VerificationTtl = TimeSpan.FromMinutes(5);
    private const int MaxOtpAttempts = 3;

    public HandleVerificationService(
        ICodeforcesClient cf,
        IMemoryCache cache,
        IUserService userService,
        IEmailService emailService)
    {
        _cf = cf;
        _cache = cache;
        _userService = userService;
        _emailService = emailService;
    }

    private static string GetCacheKey(string userId)
        => $"handle_verification:{userId}";

   
    public async Task<string> StartVerificationAsync(string userId, string handle)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(handle))
            {
                throw new CffError(
                    new BaseResponse(CffError.BAD_REQUEST, "Handle is required")
                );
            }

          
            var cfUser = await _cf.GetUserAsync(handle);

            if (cfUser == null)
            {
                throw new CffError(
                    new BaseResponse(CffError.BAD_REQUEST, "Invalid Codeforces handle")
                );
            }

   
            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                throw new CffError(
                    new BaseResponse(CffError.USER_NOT_FOUND, "User not found")
                );
            }

            if (!string.IsNullOrWhiteSpace(user.CodeforcesHandle))
            {
                throw new CffError(
                    new BaseResponse(CffError.BAD_REQUEST, "Handle already linked")
                );
            }

           if (string.IsNullOrWhiteSpace(cfUser.Email))
{
    throw new CffError(
        new BaseResponse(
            CffError.BAD_REQUEST,
            "Unable to fetch email from Codeforces. Please verify using registered email."
        )
    );
}

           
            var otp = new Random().Next(100000, 999999).ToString();

            var cacheEntry = new HandleVerificationCache
            {
                UserId = userId,
                Handle = handle,
                Otp = otp,
                Attempts = 0,
                CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            _cache.Set(GetCacheKey(userId), cacheEntry, VerificationTtl);

 
            await _emailService.SendEmailAsync(
                cfUser.Email,
                "Codeforces Handle Verification",
                $"Your OTP is {otp}. It expires in 5 minutes."
            );

            return "OTP sent to your registered email";
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(CffError.INTERNAL_ERROR, "Error starting verification"),
                ex: ex
            );
        }
    }


    public async Task<string> VerifyAsync(string userId, string otp)
    {
        try
        {
            var cacheKey = GetCacheKey(userId);

            if (!_cache.TryGetValue(cacheKey, out HandleVerificationCache verification))
            {
                throw new CffError(
                    new BaseResponse(CffError.OTP_NOT_FOUND, "OTP expired or not found")
                );
            }


            if (verification.Attempts >= MaxOtpAttempts)
            {
                _cache.Remove(cacheKey);

                throw new CffError(
                    new BaseResponse(CffError.OTP_ATTEMPTS_EXCEEDED, "Too many attempts")
                );
            }


            if (!string.Equals(verification.Otp, otp))
            {
                verification.Attempts++;
                _cache.Set(cacheKey, verification, VerificationTtl);

                throw new CffError(
                    new BaseResponse(CffError.OTP_INVALID, "Invalid OTP")
                );
            }


            var user = await _userService.GetByIdAsync(userId);

            if (user == null)
            {
                _cache.Remove(cacheKey);

                throw new CffError(
                    new BaseResponse(CffError.USER_NOT_FOUND, "User not found")
                );
            }

         
            user.CodeforcesHandle = verification.Handle;

            await _userService.SaveAsync(user);

            _cache.Remove(cacheKey);

            return "Handle verified successfully";
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(CffError.INTERNAL_ERROR, "Error verifying OTP"),
                ex: ex
            );
        }
    }
}