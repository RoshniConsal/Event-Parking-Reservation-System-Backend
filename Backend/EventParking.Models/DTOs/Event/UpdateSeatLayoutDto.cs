using System.ComponentModel.DataAnnotations;

namespace EventParking.Models.DTOs.Event;

public class UpdateSeatLayoutDto
{
    [Required]
    [MaxLength(20)]
    public string SeatLayoutType { get; set; }
        = "Theatre";
}