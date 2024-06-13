using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swps_web.Models;

	public class Device
{
    public string? Id { get; set; }

    public string? UserId { get; set; }

    [Required]
    public string? DeviceSN { get; set;}

    [Required]
    [Display(Name = "Detection Interval")]
    [Range(1, 60, ErrorMessage = "The detection interval must be greater than 1 minute.")]
    public int DetectInterval { get; set; }

    [Required]
    [Display(Name = "Pump Start Time")]
    [Column(TypeName = "decimal(6, 4)")]
    public decimal PumpStartTime { get; set; }

    [Required]
    [Display(Name = "Soil Moisture")]
    [Range(0, 65535, ErrorMessage = "Soil moisture values ​​range from 0 to 65535.")]
    public int SoilMoisture { get; set; }
}
