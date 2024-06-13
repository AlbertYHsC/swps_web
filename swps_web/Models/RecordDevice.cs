using System.ComponentModel.DataAnnotations;

namespace swps_web.Models;

public class RecordDevice
{
	public Record? Record { get; set; }

	[Display(Name = "Device SN")]
	public string? DeviceSN { get; set; }
}
