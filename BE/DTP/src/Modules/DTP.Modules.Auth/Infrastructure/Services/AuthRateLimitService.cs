using DTP.Modules.Auth.Application.Abstractions.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Auth.Infrastructure.Services
{
    public class AuthRateLimitService : IAuthRateLimitService
    {
        private readonly IDatabase _redis;

        private const int MaxLoginFailByEmail = 5;
        private const int MaxLoginFailByIp = 20;

        private static readonly TimeSpan LoginBlockTtl = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan LoginFailTtl = TimeSpan.FromMinutes(15);

        private const int MaxOtpByTarget = 3;
        private const int MaxOtpByIp = 10;

        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(10);

        private const int MaxRegisterByEmail = 3;
        private const int MaxRegisterByIp = 5;

        private static readonly TimeSpan RegisterTtl = TimeSpan.FromMinutes(10);

        private const int MaxOtpVerifyFailByTarget = 5;
        private const int MaxOtpVerifyFailByIp = 30;

        private static readonly TimeSpan OtpVerifyFailTtl = TimeSpan.FromMinutes(15);

        public AuthRateLimitService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<bool> IsLoginBlockedAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            email = NormalizeEmail(email);
            ipAddress = NormalizeIp(ipAddress);

            var emailBlocked = await _redis.KeyExistsAsync(
                AuthRateLimitKeys.LoginBlockedEmail(email));

            var ipBlocked = await _redis.KeyExistsAsync(
                AuthRateLimitKeys.LoginBlockedIp(ipAddress));

            return emailBlocked || ipBlocked;
        }

        public async Task RegisterLoginFailedAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            email = NormalizeEmail(email);
            ipAddress = NormalizeIp(ipAddress);

            var emailFailKey = AuthRateLimitKeys.LoginFailEmail(email);
            var ipFailKey = AuthRateLimitKeys.LoginFailIp(ipAddress);

            var emailFails = await _redis.StringIncrementAsync(emailFailKey);
            var ipFails = await _redis.StringIncrementAsync(ipFailKey);

            if (emailFails == 1)
                await _redis.KeyExpireAsync(emailFailKey, LoginFailTtl);

            if (ipFails == 1)
                await _redis.KeyExpireAsync(ipFailKey, LoginFailTtl);

            if (emailFails >= MaxLoginFailByEmail)
            {
                await _redis.StringSetAsync(
                    AuthRateLimitKeys.LoginBlockedEmail(email),
                    "1",
                    LoginBlockTtl);
            }

            if (ipFails >= MaxLoginFailByIp)
            {
                await _redis.StringSetAsync(
                    AuthRateLimitKeys.LoginBlockedIp(ipAddress),
                    "1",
                    LoginBlockTtl);
            }
        }

        public async Task RegisterLoginSuccessAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            email = NormalizeEmail(email);

            await _redis.KeyDeleteAsync(AuthRateLimitKeys.LoginFailEmail(email));
            await _redis.KeyDeleteAsync(AuthRateLimitKeys.LoginBlockedEmail(email));

            // Không xóa IP fail vì 1 IP có thể đang brute-force nhiều tài khoản.
        }


        public async Task<bool> IsOtpBlockedAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            target = NormalizeTarget(target);
            ipAddress = NormalizeIp(ipAddress);

            var sendTargetCount = await GetLongAsync(AuthRateLimitKeys.OtpTargetKey(target));
            var sendIpCount = await GetLongAsync(AuthRateLimitKeys.OtpIpKey(ipAddress));

            if (sendTargetCount >= MaxOtpByTarget)
                return true;

            if (sendIpCount >= MaxOtpByIp)
                return true;

            var verifyTargetCount = await GetLongAsync(AuthRateLimitKeys.OtpVerifyTargetKey(target));
            var verifyIpCount = await GetLongAsync(AuthRateLimitKeys.OtpVerifyIpKey(ipAddress));

            if (verifyTargetCount >= MaxOtpVerifyFailByTarget)
                return true;

            if (verifyIpCount >= MaxOtpVerifyFailByIp)
                return true;

            return false;
        }



        public async Task RegisterOtpSentAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            target = NormalizeTarget(target);
            ipAddress = NormalizeIp(ipAddress);

            var targetKey = AuthRateLimitKeys.OtpTarget(target);
            var ipKey = AuthRateLimitKeys.OtpIp(ipAddress);

            var targetCount = await _redis.StringIncrementAsync(targetKey);
            var ipCount = await _redis.StringIncrementAsync(ipKey);

            if (targetCount == 1)
                await _redis.KeyExpireAsync(targetKey, OtpTtl);

            if (ipCount == 1)
                await _redis.KeyExpireAsync(ipKey, OtpTtl);
        }

        public async Task<bool> IsRegisterBlockedAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            email = NormalizeEmail(email);
            ipAddress = NormalizeIp(ipAddress);

            var emailCount = await GetIntAsync(AuthRateLimitKeys.RegisterEmail(email));
            var ipCount = await GetIntAsync(AuthRateLimitKeys.RegisterIp(ipAddress));

            return emailCount >= MaxRegisterByEmail || ipCount >= MaxRegisterByIp;
        }

        public async Task RegisterRegisterAttemptAsync(
            string email,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            email = NormalizeEmail(email);
            ipAddress = NormalizeIp(ipAddress);

            var emailKey = AuthRateLimitKeys.RegisterEmail(email);
            var ipKey = AuthRateLimitKeys.RegisterIp(ipAddress);

            var emailCount = await _redis.StringIncrementAsync(emailKey);
            var ipCount = await _redis.StringIncrementAsync(ipKey);

            if (emailCount == 1)
                await _redis.KeyExpireAsync(emailKey, RegisterTtl);

            if (ipCount == 1)
                await _redis.KeyExpireAsync(ipKey, RegisterTtl);
        }


        public async Task RegisterOtpVerifyFailedAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            target = NormalizeTarget(target);
            ipAddress = NormalizeIp(ipAddress);

            var targetKey = AuthRateLimitKeys.OtpVerifyTargetKey(target);
            var ipKey = AuthRateLimitKeys.OtpVerifyIpKey(ipAddress);

            var targetCount = await _redis.StringIncrementAsync(targetKey);
            var ipCount = await _redis.StringIncrementAsync(ipKey);

            if (targetCount == 1)
                await _redis.KeyExpireAsync(targetKey, OtpVerifyFailTtl);

            if (ipCount == 1)
                await _redis.KeyExpireAsync(ipKey, OtpVerifyFailTtl);
        }

        public async Task RegisterOtpVerifySuccessAsync(
            string target,
            string ipAddress,
            CancellationToken cancellationToken = default)
        {
            target = NormalizeTarget(target);
            ipAddress = NormalizeIp(ipAddress);

            await _redis.KeyDeleteAsync(AuthRateLimitKeys.OtpVerifyTargetKey(target));
            await _redis.KeyDeleteAsync(AuthRateLimitKeys.OtpVerifyIpKey(ipAddress));
        }

        private async Task<int> GetIntAsync(string key)
        {
            var value = await _redis.StringGetAsync(key);

            if (!value.HasValue)
                return 0;

            return int.TryParse(value.ToString(), out var result)
                ? result
                : 0;
        }

        private static string NormalizeEmail(string email)
        {
            return string.IsNullOrWhiteSpace(email)
                ? "unknown"
                : email.Trim().ToLowerInvariant();
        }

        private static string NormalizeTarget(string target)
        {
            return string.IsNullOrWhiteSpace(target)
                ? "unknown"
                : target.Trim().ToLowerInvariant();
        }

        private static string NormalizeIp(string ip)
        {
            return string.IsNullOrWhiteSpace(ip)
                ? "unknown"
                : ip.Trim();
        }


        private async Task<long> GetLongAsync(string key)
        {
            var value = await _redis.StringGetAsync(key);

            if (!value.HasValue)
                return 0;

            return long.TryParse(value.ToString(), out var count)
                ? count
                : 0;
        }
    }

    public static class AuthRateLimitKeys
    {
        public static string LoginFailEmail(string email)
            => $"auth:login:fail:email:{email}";

        public static string LoginFailIp(string ip)
            => $"auth:login:fail:ip:{ip}";

        public static string LoginBlockedEmail(string email)
            => $"auth:login:block:email:{email}";

        public static string LoginBlockedIp(string ip)
            => $"auth:login:block:ip:{ip}";

        public static string OtpTarget(string target)
            => $"auth:otp:target:{target}";

        public static string OtpIp(string ip)
            => $"auth:otp:ip:{ip}";

        public static string RegisterEmail(string email)
            => $"auth:register:email:{email}";

        public static string RegisterIp(string ip)
            => $"auth:register:ip:{ip}";


        public static string OtpVerifyTargetKey(string target)
    => $"auth:otp:verify:target:{target}";

        public static string OtpVerifyIpKey(string ip)
            => $"auth:otp:verify:ip:{ip}";

        public static string OtpTargetKey(string target)
    => $"auth:otp:send:target:{target}";

        public static string OtpIpKey(string ip)
            => $"auth:otp:send:ip:{ip}";

    }
}
