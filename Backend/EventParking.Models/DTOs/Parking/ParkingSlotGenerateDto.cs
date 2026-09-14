using System.ComponentModel.DataAnnotations;

namespace EventParking.Models.DTOs.Parking;

public class ParkingSlotGenerateDto
{
    [Range(1, 500)]
    public int NumberOfSlots { get; set; } = 100;

    [MaxLength(20)]
    public string Prefix { get; set; } = "P";

    [Range(1, int.MaxValue)]
    public int StartNumber { get; set; } = 1;

    [Required]
    [MaxLength(100)]
    public string Zone { get; set; } = "A";

    [Range(0, double.MaxValue)]
    public decimal Fee { get; set; }
}