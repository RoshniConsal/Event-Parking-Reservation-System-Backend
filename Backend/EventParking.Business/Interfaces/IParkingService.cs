using EventParking.Models.DTOs.Parking;

namespace EventParking.Business.Interfaces;

public interface IParkingService
{
    Task<List<ParkingSlotDto>> GetByEventIdAsync(
        int eventId,
        bool availableOnly = false);

    Task<ParkingSlotDto> GetByIdAsync(
        int id);

    Task<ParkingSlotDto> CreateAsync(
        int eventId,
        ParkingSlotCreateDto dto);

    Task<List<ParkingSlotDto>> GenerateAsync(
        int eventId,
        ParkingSlotGenerateDto dto);

    Task<ParkingSlotDto> UpdateAsync(
        int id,
        ParkingSlotUpdateDto dto);

    Task DeleteAsync(
        int id);
}