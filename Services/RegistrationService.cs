using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using CFFFusions.Models;
using Cff.Error.Exceptions;
using Cff.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CFFFusions.Services;

public class RegistrationService : IRegistrationService
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;

    private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(10);
    private const int MaxOtpAttempts = 5;

    public RegistrationService(
        IUserService userService,
        IEmailService emailService,
        IMemoryCache cache)
    {
        _userService = userService;
        _emailService = emailService;
        _cache = cache;
    }

    // ======================= REGISTER =======================

    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        try
        {
            
            CffError.AssertOrThrow(
                IsValidEmail(request.Email),
                CffError.INVALID_EMAIL,
                "Invalid email format"
            );

            var user = await _userService.GetByEmailAsync(request.Email);

          
            if (user != null && user.IsVerified)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.EMAIL_ALREADY_VERIFIED,
                        "Email already registered and verified"
                    )
                );
            }

           
            if (user == null)
            {
                user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    Password = HashPassword(request.Password),
                    IsVerified = false
                };

                await _userService.CreateAsync(user);
            }

            
            var otp = GenerateOtp();
            var cacheKey = GetOtpCacheKey(request.Email);

            var otpEntry = new OtpCacheEntry
            {
                Otp = otp,
                Attempts = 0
            };

            _cache.Set(cacheKey, otpEntry, OtpTtl);
            Console.WriteLine("EVERTINGING FINE UPTO HERE");

           
            await _emailService.SendEmailAsync(
                request.Email,
                "Verify your CFFFusions account",
                $"""
                <h2>Email Verification</h2>
                <p>Your OTP is:</p>
                <h1>{otp}</h1>
                <p>This OTP is valid for 10 minutes.</p>
                """
            );

            return "OTP sent to email";
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error during registration"
                ),
                ex: ex
            );
        }
    }

    // ======================= VERIFY OTP =======================

    public async Task<string> VerifyOtpAsync(VerifyOtpRequest request)
    {
        try
        {
            var cacheKey = GetOtpCacheKey(request.Email);

        
            if (!_cache.TryGetValue<OtpCacheEntry>(cacheKey, out var otpEntry))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.OTP_NOT_FOUND,
                        "OTP expired or not found"
                    )
                );
            }

            
            if (otpEntry.Attempts >= MaxOtpAttempts)
            {
                _cache.Remove(cacheKey);
                throw new CffError(
                    new BaseResponse(
                        CffError.OTP_ATTEMPTS_EXCEEDED,
                        "OTP attempts exceeded"
                    )
                );
            }

           
            if (!string.Equals(otpEntry.Otp, request.Otp))
            {
                otpEntry.Attempts++;
                _cache.Set(cacheKey, otpEntry, OtpTtl);

                throw new CffError(
                    new BaseResponse(
                        CffError.OTP_INVALID,
                        "Invalid OTP"
                    )
                );
            }

           
            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _cache.Remove(cacheKey);
                throw new CffError(
                    new BaseResponse(
                        CffError.USER_NOT_FOUND,
                        "User not found"
                    )
                );
            }

            user.IsVerified = true;
            await _userService.SaveAsync(user);

            _cache.Remove(cacheKey);
            return "Email verified successfully";
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error verifying OTP"
                ),
                ex: ex
            );
        }
    }


    private static string GetOtpCacheKey(string email)
        => $"otp:{email.ToLowerInvariant()}";

    private static string GenerateOtp()
        => RandomNumberGenerator.GetInt32(100000, 999999).ToString();

    private static bool IsValidEmail(string email)
        => Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(
            sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
        );
    }
}
