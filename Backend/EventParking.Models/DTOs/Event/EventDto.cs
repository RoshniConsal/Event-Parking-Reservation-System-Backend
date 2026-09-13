namespace EventParking.Models.DTOs.Event;

public class EventDto
{
    public int Id { get; set; }

    public string Name { get; set; }
        = string.Empty;

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public string VenueName { get; set; }
        = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; }
        = string.Empty;

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public decimal TicketPrice { get; set; }

    public decimal ParkingFee { get; set; }

    public int Capacity { get; set; }

    public string SeatLayoutType { get; set; }
        = "Theatre";
}