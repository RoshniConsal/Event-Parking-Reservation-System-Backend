using EventParking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventParking.DataAccess.Configurations;

public class EventConfiguration
    : IEntityTypeConfiguration<Event>
{
    public void Configure(
        EntityTypeBuilder<Event> builder)
    {
        builder.ToTable(
            "Events",
            table =>
            {
                table.HasCheckConstraint(
                    "CK_Events_Capacity",
                    "[Capacity] > 0");

                table.HasCheckConstraint(
                    "CK_Events_TicketPrice",
                    "[TicketPrice] >= 0");

                table.HasCheckConstraint(
                    "CK_Events_ParkingFee",
                    "[ParkingFee] >= 0");

                table.HasCheckConstraint(
                    "CK_Events_DateTime",
                    "[EndDateTime] > [StartDateTime]");
            });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.TicketPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.ParkingFee)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.Property(x => x.SeatLayoutType)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Theatre");

        builder.Property(x => x.StartDateTime)
            .IsRequired();

        builder.Property(x => x.EndDateTime)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x =>
            new
            {
                x.VenueId,
                x.StartDateTime,
                x.EndDateTime
            });

        builder.HasIndex(x => x.CategoryId);

        builder.HasIndex(x => x.Name);

        builder.HasOne(x => x.Venue)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}