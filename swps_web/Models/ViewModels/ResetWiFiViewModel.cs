using System.ComponentModel.DataAnnotations;

namespace swps_web.Models.ViewModels
{
	public class ResetWiFiViewModel
	{
		[Required]
		public string? DeviceSN { get; set; }

		[Required]
		[StringLength(32)]
		[Display(Name = "WiFi SSID")]
		public string? WiFiSsid { get; set; }

		[Required]
		[StringLength(64)]
		[DataType(DataType.Password)]
		[Display(Name = "WiFi Password")]
		public string? WiFiPassword { get; set; }

		[Required]
		[StringLength(64)]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm Password")]
		[Compare("WiFiPassword", ErrorMessage = "The password and confirmation password do not match.")]
		public string? WiFiConfirmPassword { get; set; }
	}
}
