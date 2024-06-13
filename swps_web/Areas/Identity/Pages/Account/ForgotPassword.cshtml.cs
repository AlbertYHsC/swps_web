// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using swps_web.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace swps_web.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly swps_UserManager<swps_webUser> _userManager;

        public ForgotPasswordModel(swps_UserManager<swps_webUser> userManager)
        {
            _userManager = userManager;
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
            public string Username { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(60, MinimumLength = 3)]
            public string DeviceSK { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IdentityResult result;
                var errors = new List<IdentityError>();

                var deviceSN = _userManager.ConvertDeviceSKToDeviceSN(Input.DeviceSK);
                var user = await _userManager.FindByDeviceSNAsync(deviceSN);
                if (user == null)
                {
                    errors.Add(new IdentityError { Description = $"Invalid Username '{Input.Username}' or Device SK '{Input.DeviceSK}'." });
                }
                else
                {
                    result = await _userManager.VerifyRecoveryCodeAsync(user, Input.Username, Input.DeviceSK);

                    if (result.Succeeded)
                    {
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var userName = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(Input.Username));

                        return RedirectToPage("./ResetPassword", new {code = code, userName = userName});
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

            return Page();
        }
    }
}
