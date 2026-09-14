using EventParking.Business.Exceptions;
using EventParking.Business.Interfaces;
using EventParking.DataAccess.Interfaces;
using EventParking.Models.DTOs.Parking;
using EventParking.Models.Entities;
using EventParking.Models.Enums;

namespace EventParking.Business.Services;

public class ParkingService : IParkingService
{
    private readonly IParkingRepository _parkingRepository;
    private readonly IEventRepository _eventRepository;


    public ParkingService(
        IParkingRepository parkingRepository,
        IEventRepository eventRepository)
    {
        _parkingRepository = parkingRepository;
        _eventRepository = eventRepository;
    }


    /* =====================================================
       GET BY EVENT
       ===================================================== */

    public async Task<List<ParkingSlotDto>> GetByEventIdAsync(
        int eventId,
        bool availableOnly = false)
    {
        var eventEntity =
            await _eventRepository.GetByIdAsync(eventId);

        if (eventEntity == null)
        {
            throw new NotFoundException(
                $"Event with ID {eventId} was not found.");
        }


        var parkingSlots =
            availableOnly

                ? await _parkingRepository
                    .GetAvailableByEventIdAsync(eventId)

                : await _parkingRepository
                    .GetByEventIdAsync(eventId);


        return parkingSlots
            .Select(MapToDto)
            .ToList();
    }


    /* =====================================================
       GET BY ID
       ===================================================== */

    public async Task<ParkingSlotDto> GetByIdAsync(
        int id)
    {
        var parkingSlot =
            await _parkingRepository.GetByIdAsync(id);


        if (parkingSlot == null)
        {
            throw new NotFoundException(
                $"Parking slot with ID {id} was not found.");
        }


        return MapToDto(parkingSlot);
    }


    /* =====================================================
       CREATE SINGLE PARKING SLOT
       ===================================================== */

    public async Task<ParkingSlotDto> CreateAsync(
        int eventId,
        ParkingSlotCreateDto dto)
    {
        var eventEntity =
            await _eventRepository.GetByIdAsync(eventId);


        if (eventEntity == null)
        {
            throw new NotFoundException(
                $"Event with ID {eventId} was not found.");
        }


        ValidateCreateDto(dto);


        var normalizedSlotNumber =
            dto.SlotNumber.Trim();


        var exists =
            await _parkingRepository
                .SlotNumberExistsAsync(
                    eventId,
                    normalizedSlotNumber);


        if (exists)
        {
            throw new ConflictException(
                $"Parking slot '{normalizedSlotNumber}' already exists for this event.");
        }


        var parkingSlot =
            new ParkingSlot
            {
                EventId = eventId,

                SlotNumber =
                    normalizedSlotNumber,

                Zone =
                    dto.Zone.Trim(),

                Fee =
                    dto.Fee,

                Status =
                    ParkingSlotStatus.Available
            };


        await _parkingRepository
            .AddAsync(parkingSlot);


        await _parkingRepository
            .SaveChangesAsync();


        return MapToDto(
            parkingSlot);
    }


    /* =====================================================
       GENERATE PARKING SLOTS
       ===================================================== */

    public async Task<List<ParkingSlotDto>> GenerateAsync(
        int eventId,
        ParkingSlotGenerateDto dto)
    {
        var eventEntity =
            await _eventRepository
                .GetByIdAsync(eventId);


        if (eventEntity == null)
        {
            throw new NotFoundException(
                $"Event with ID {eventId} was not found.");
        }


        ValidateGenerateDto(dto);


        var prefix =
            dto.Prefix?.Trim()
            ?? string.Empty;


        var zone =
            dto.Zone.Trim();


        /*
         * Get all existing slots first.
         * This avoids generating duplicate
         * slot numbers.
         */

        var existingSlots =
            await _parkingRepository
                .GetByEventIdAsync(eventId);


        var existingNumbers =
            new HashSet<string>(
                existingSlots.Select(
                    slot =>
                        slot.SlotNumber),
                StringComparer.OrdinalIgnoreCase);


        var finalNumber =
            dto.StartNumber
            +
            dto.NumberOfSlots
            -
            1;


        /*
         * Example:
         *
         * NumberOfSlots = 100
         * StartNumber = 1
         *
         * P001
         * P002
         * ...
         * P100
         */

        var numberPadding =
            Math.Max(
                2,
                finalNumber
                    .ToString()
                    .Length);


        var generatedSlots =
            new List<ParkingSlot>();


        /*
         * First validate every generated
         * slot number before inserting.
         *
         * If one duplicate exists,
         * nothing will be generated.
         */

        var generatedNumbers =
            new List<string>();


        for (
            var index = 0;
            index < dto.NumberOfSlots;
            index++)
        {
            var numericPart =
                dto.StartNumber
                +
                index;


            var slotNumber =
                $"{prefix}{numericPart.ToString($"D{numberPadding}")}";


            if (
                slotNumber.Length > 50)
            {
                throw new ValidationException(
                    "Generated parking slot number cannot exceed 50 characters.");
            }


            if (
                existingNumbers.Contains(
                    slotNumber))
            {
                throw new ConflictException(
                    $"Parking slot '{slotNumber}' already exists for this event.");
            }


            existingNumbers.Add(
                slotNumber);


            generatedNumbers.Add(
                slotNumber);
        }


        /*
         * Create all parking slot entities.
         */

        foreach (
            var slotNumber in generatedNumbers)
        {
            var parkingSlot =
                new ParkingSlot
                {
                    EventId =
                        eventId,

                    SlotNumber =
                        slotNumber,

                    Zone =
                        zone,

                    Fee =
                        dto.Fee,

                    Status =
                        ParkingSlotStatus.Available
                };


            await _parkingRepository
                .AddAsync(
                    parkingSlot);


            generatedSlots.Add(
                parkingSlot);
        }


        /*
         * Save ALL slots with a single
         * SaveChanges call.
         */

        await _parkingRepository
            .SaveChangesAsync();


        return generatedSlots
            .Select(
                MapToDto)
            .ToList();
    }


    /* =====================================================
       UPDATE
       ===================================================== */

    public async Task<ParkingSlotDto> UpdateAsync(
        int id,
        ParkingSlotUpdateDto dto)
    {
        var parkingSlot =
            await _parkingRepository.GetByIdAsync(id);


        if (parkingSlot == null)
        {
            throw new NotFoundException(
                $"Parking slot with ID {id} was not found.");
        }


        ValidateUpdateDto(
            dto);


        var hasActiveReservation =
            await _parkingRepository
                .HasActiveReservationAsync(id);


        if (hasActiveReservation)
        {
            throw new ConflictException(
                "This parking slot has an active reservation and cannot be modified.");
        }


        var normalizedSlotNumber =
            dto.SlotNumber.Trim();


        var duplicate =
            await _parkingRepository
                .SlotNumberExistsAsync(
                    parkingSlot.EventId,
                    normalizedSlotNumber,
                    parkingSlot.Id);


        if (duplicate)
        {
            throw new ConflictException(
                $"Parking slot '{normalizedSlotNumber}' already exists for this event.");
        }


        parkingSlot.SlotNumber =
            normalizedSlotNumber;


        parkingSlot.Zone =
            dto.Zone.Trim();


        parkingSlot.Fee =
            dto.Fee;


        parkingSlot.Status =
            dto.Status;


        _parkingRepository.Update(
            parkingSlot);


        await _parkingRepository
            .SaveChangesAsync();


        return MapToDto(
            parkingSlot);
    }


    /* =====================================================
       DELETE
       ===================================================== */

    public async Task DeleteAsync(
        int id)
    {
        var parkingSlot =
            await _parkingRepository
                .GetByIdAsync(id);


        if (parkingSlot == null)
        {
            throw new NotFoundException(
                $"Parking slot with ID {id} was not found.");
        }


        var hasActiveReservation =
            await _parkingRepository
                .HasActiveReservationAsync(id);


        if (hasActiveReservation)
        {
            throw new ConflictException(
                "This parking slot has an active reservation and cannot be deleted.");
        }


        _parkingRepository.Delete(
            parkingSlot);


        await _parkingRepository
            .SaveChangesAsync();
    }


    /* =====================================================
       CREATE VALIDATION
       ===================================================== */

    private static void ValidateCreateDto(
        ParkingSlotCreateDto dto)
    {
        if (
            string.IsNullOrWhiteSpace(
                dto.SlotNumber))
        {
            throw new ValidationException(
                "Parking slot number is required.");
        }


        if (
            string.IsNullOrWhiteSpace(
                dto.Zone))
        {
            throw new ValidationException(
                "Parking zone is required.");
        }


        if (
            dto.Fee < 0)
        {
            throw new ValidationException(
                "Parking fee cannot be negative.");
        }
    }


    /* =====================================================
       GENERATE VALIDATION
       ===================================================== */

    private static void ValidateGenerateDto(
        ParkingSlotGenerateDto dto)
    {
        if (
            dto.NumberOfSlots <= 0)
        {
            throw new ValidationException(
                "Number of parking slots must be greater than zero.");
        }


        if (
            dto.NumberOfSlots > 500)
        {
            throw new ValidationException(
                "A maximum of 500 parking slots can be generated at one time.");
        }


        if (
            dto.StartNumber <= 0)
        {
            throw new ValidationException(
                "Start number must be greater than zero.");
        }


        if (
            string.IsNullOrWhiteSpace(
                dto.Zone))
        {
            throw new ValidationException(
                "Parking zone is required.");
        }


        if (
            dto.Fee < 0)
        {
            throw new ValidationException(
                "Parking fee cannot be negative.");
        }


        if (
            dto.Prefix != null
            &&
            dto.Prefix.Trim().Length > 20)
        {
            throw new ValidationException(
                "Parking slot prefix cannot exceed 20 characters.");
        }
    }


    /* =====================================================
       UPDATE VALIDATION
       ===================================================== */

    private static void ValidateUpdateDto(
        ParkingSlotUpdateDto dto)
    {
        if (
            string.IsNullOrWhiteSpace(
                dto.SlotNumber))
        {
            throw new ValidationException(
                "Parking slot number is required.");
        }


        if (
            string.IsNullOrWhiteSpace(
                dto.Zone))
        {
            throw new ValidationException(
                "Parking zone is required.");
        }


        if (
            dto.Fee < 0)
        {
            throw new ValidationException(
                "Parking fee cannot be negative.");
        }


        if (
            dto.Status !=
                ParkingSlotStatus.Available
            &&
            dto.Status !=
                ParkingSlotStatus.Unavailable)
        {
            throw new ValidationException(
                "Administrator can only manually set a parking slot as Available or Unavailable.");
        }
    }


    /* =====================================================
       MAP
       ===================================================== */

    private static ParkingSlotDto MapToDto(
        ParkingSlot parkingSlot)
    {
        return new ParkingSlotDto
        {
            Id =
                parkingSlot.Id,

            EventId =
                parkingSlot.EventId,

            SlotNumber =
                parkingSlot.SlotNumber,

            Zone =
                parkingSlot.Zone
                ?? string.Empty,

            Fee =
                parkingSlot.Fee
                ?? 0m,

            Status =
                parkingSlot.Status
        };
    }
}