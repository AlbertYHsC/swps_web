using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace swps_web.Models;

public class Record
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? DeviceId { get; set; }

    [Column(TypeName = "decimal(8, 4)")]
    public decimal Temperature { get; set; }

    [Column(TypeName = "decimal(8, 4)")]
    public decimal Humidity { get; set; }

    [Column(TypeName = "decimal(8, 4)")]
    public decimal Pressure { get; set; }

    [Display(Name = "Light Intensity")]
    public int RawValue0 { get; set; }

    [Display(Name = "Water Level")]
    public int RawValue1 { get; set; }

    [Display(Name = "Soil Moisture")]
    public int RawValue2 { get; set;}

    public int RawValue3 { get; set;}

    [Column(TypeName = "decimal(6, 4)")]
    public decimal Voltage0 { get; set; }

    [Column(TypeName = "decimal(6, 4)")]
    public decimal Voltage1 { get; set; }

    [Column(TypeName = "decimal(6, 4)")]
    public decimal Voltage2 { get; set; }

    [Column(TypeName = "decimal(6, 4)")]
    public decimal Voltage3 { get; set; }

    [Display(Name = "Detect Time ")]
    [DataType(DataType.DateTime)]
    public DateTime DetectTime { get; set; }

    [Display(Name = "Pump Start Time")]
    [Column(TypeName = "decimal(6, 4)")]
    public decimal PumpStartTime { get; set; }
}
