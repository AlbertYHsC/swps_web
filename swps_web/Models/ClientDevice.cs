using System.ComponentModel.DataAnnotations;

namespace swps_web.Models;

public class ClientDevice
{
    [Display(Name = "Device SN")]
    public string? DeviceSN { get; set; }

    public bool Status { get; set; }

    public bool Registered { get; set; }
}
