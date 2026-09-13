namespace EventParking.Models.Entities;

public class Event
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public int CategoryId { get; set; }

    public DateTime StartDateTime { get; set; }

    public DateTime EndDateTime { get; set; }

    public decimal TicketPrice { get; set; }

    public decimal ParkingFee { get; set; }

    public int Capacity { get; set; }

    public string SeatLayoutType { get; set; }
        = "Theatre";

    public DateTime CreatedAt { get; set; }
        = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Venue? Venue { get; set; }

    public EventCategory? Category { get; set; }

    public ICollection<Seat> Seats { get; set; }
        = new List<Seat>();

    public ICollection<ParkingSlot> ParkingSlots { get; set; }
        = new List<ParkingSlot>();

    public ICollection<Booking> Bookings { get; set; }
        = new List<Booking>();

    public ICollection<Notification> Notifications { get; set; }
        = new List<Notification>();
}