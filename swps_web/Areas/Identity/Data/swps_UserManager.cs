using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace swps_web.Areas.Identity.Data;

public class swps_UserManager<TUser> : UserManager<TUser> where TUser : IdentityUser
{
    public swps_UserManager(
        IUserStore<TUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<TUser> passwordHasher,
        IEnumerable<IUserValidator<TUser>> userValidators,
        IEnumerable<IPasswordValidator<TUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<TUser>> logger)
        : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }

    public virtual async Task<IdentityResult> SetDeviceSNAsync(TUser user, string? deviceSN)
    {
        ThrowIfDisposed();
        return await SetEmailAsync(user, deviceSN);
    }

    public virtual async Task<string?> GetDeviceSNAsync(TUser user)
    {
        ThrowIfDisposed();
        return await GetEmailAsync(user);
    }
    public virtual async Task<TUser?> FindByDeviceSNAsync(string? deviceSN)
    {
        ThrowIfDisposed();
        if (deviceSN != null)
        {
            return await FindByEmailAsync(deviceSN);
        }
        else
        {
            return null;
        }
    }

    public virtual string? ConvertDeviceSKToDeviceSN(string deviceSK)
    {
        string? deviceSN;
        
        string[] strArrSK = deviceSK.Split('-');
        bool[] checkSK = strArrSK.Select(x => Byte.TryParse(x, NumberStyles.AllowHexSpecifier, null, out byte b)).ToArray();

        if (checkSK.All(x => x))
        {
            deviceSN = "SWPS" + String.Join("", strArrSK[^4..]).ToUpper();
            return deviceSN;
        }
        else
        {
            deviceSN = null;
        }
        
        return deviceSN;
    }

    public virtual string ConvertDeviceSKToRecoveryCode(string salt, string deviceSK)
    {
        string[] strArrSK = deviceSK.Split("-");
        deviceSK = String.Join("", strArrSK);

        string hashedSK = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: deviceSK,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        return hashedSK;
    }


    public virtual async Task<IdentityResult> SetRecoveryCodeAsync(TUser user, string userName, string deviceSK)
    {
        ThrowIfDisposed();

        string hashedSK = ConvertDeviceSKToRecoveryCode(userName, deviceSK);
        
        return await SetPhoneNumberAsync(user, hashedSK);
    }

    public virtual async Task<IdentityResult> VerifyRecoveryCodeAsync(TUser user, string userName, string deviceSK)
    {
        ThrowIfDisposed();

        string hashedSK = ConvertDeviceSKToRecoveryCode(userName, deviceSK);
        var recoveryCode = await GetPhoneNumberAsync(user);
        if (hashedSK == recoveryCode)
        {
            return IdentityResult.Success;
        }
        else
        {
            return IdentityResult.Failed(new IdentityError { Description = $"Invalid Username '{userName}' or Device SK '{deviceSK}'." });
        }
    }
}
