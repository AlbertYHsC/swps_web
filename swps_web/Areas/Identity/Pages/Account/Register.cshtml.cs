// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using swps_web.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace swps_web.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<swps_webUser> _signInManager;
        private readonly swps_UserManager<swps_webUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            swps_UserManager<swps_webUser> userManager,
            SignInManager<swps_webUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(60, MinimumLength = 3)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(60, MinimumLength = 3)]
            [Display(Name = "Device SK")]
            public string DeviceSK { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                IdentityResult result;
                var errors = new List<IdentityError>();
                var user = CreateUser();

                var deviceSN = _userManager.ConvertDeviceSKToDeviceSN(Input.DeviceSK);
                var checkSN = await _userManager.FindByDeviceSNAsync(deviceSN);
                if (deviceSN == null)
                {
                    errors.Add(new IdentityError { Description = $"Invalid Device SK '{Input.DeviceSK}'." });
                }
                else if (checkSN != null)
                {
                    errors.Add(new IdentityError { Description = $"Device SK '{Input.DeviceSK}' is already registered." });
                }
                else
                {
                    await _userManager.SetUserNameAsync(user, Input.Username);
                    await _userManager.SetDeviceSNAsync(user, deviceSN);
                    await _userManager.SetRecoveryCodeAsync(user, Input.Username, Input.DeviceSK);
                    result = await _userManager.CreateAsync(user, Input.Password);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        errors.AddRange(result.Errors);
                    }
                }

                result = IdentityResult.Failed(errors.ToArray());
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private swps_webUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<swps_webUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(swps_webUser)}'. " +
                    $"Ensure that '{nameof(swps_webUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }
    }
}
