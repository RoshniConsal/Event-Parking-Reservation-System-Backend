using EventParking.Models.DTOs.Event;

namespace EventParking.Business.Interfaces;

public interface IEventService
{
    Task<List<EventDto>> GetAllAsync(
        string? name,
        DateOnly? date,
        int? venueId,
        int? categoryId);

    Task<EventDto> GetByIdAsync(
        int id);

    Task<EventDto> CreateAsync(
        EventCreateDto dto);

    Task<EventDto> UpdateAsync(
        int id,
        EventUpdateDto dto);

    Task<EventDto> UpdateSeatLayoutAsync(
        int id,
        UpdateSeatLayoutDto dto);

    Task DeleteAsync(
        int id);
}