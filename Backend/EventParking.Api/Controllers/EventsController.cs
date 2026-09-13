using EventParking.Business.Exceptions;
using EventParking.Business.Interfaces;
using EventParking.Models.DTOs.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventParking.Api.Controllers;

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventService
        _eventService;


    public EventsController(
        IEventService eventService)
    {
        _eventService =
            eventService;
    }


    // GET /api/events
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? name,
        [FromQuery] DateOnly? date,
        [FromQuery] int? venueId,
        [FromQuery] int? categoryId)
    {
        var events =
            await _eventService
                .GetAllAsync(
                    name,
                    date,
                    venueId,
                    categoryId);

        return Ok(events);
    }


    // GET /api/events/1
    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult>
        GetById(
            int id)
    {
        try
        {
            var eventDto =
                await _eventService
                    .GetByIdAsync(id);

            return Ok(eventDto);
        }
        catch (
            NotFoundException ex)
        {
            return NotFound(
                new
                {
                    message =
                        ex.Message
                });
        }
    }


    // POST /api/events
    [Authorize(
        Roles = "Administrator")]
    [HttpPost]
    public async Task<IActionResult>
        Create(
            [FromBody]
            EventCreateDto dto)
    {
        try
        {
            var eventDto =
                await _eventService
                    .CreateAsync(dto);


            return CreatedAtAction(
                nameof(GetById),
                new
                {
                    id =
                        eventDto.Id
                },
                eventDto);
        }
        catch (
            NotFoundException ex)
        {
            return NotFound(
                new
                {
                    message =
                        ex.Message
                });
        }
        catch (
            ValidationException ex)
        {
            return BadRequest(
                new
                {
                    message =
                        ex.Message
                });
        }
        catch (
            ConflictException ex)
        {
            return Conflict(
                new
                {
                    message =
                        ex.Message
                });
        }
    }


    // PUT /api/events/1
    [Authorize(
        Roles = "Administrator")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult>
        Update(
            int id,
            [FromBody]
            EventUpdateDto dto)
    {
        try
        {
            var eventDto =
                await _eventService
                    .UpdateAsync(
                        id,
                        dto);


            return Ok(eventDto);
        }
        catch (
            NotFoundException ex)
        {
            return NotFound(
                new
                {
                    message =
                        ex.Message
                });
        }
        catch (
            ValidationException ex)
        {
            return BadRequest(
                new
                {
                    message =
                        ex.Message
                });
        }
        catch (
            ConflictException ex)
        {
            return Conflict(
                new
                {
                    message =
                        ex.Message
                });
        }
    }


    // =========================================================
    // UPDATE SEAT LAYOUT
    //
    // PATCH /api/events/1/seat-layout
    // =========================================================

    [Authorize(
        Roles = "Administrator")]
    [HttpPatch(
        "{id:int}/seat-layout")]
    public async Task<IActionResult>
        UpdateSeatLayout(
            int id,
            [FromBody]
            UpdateSeatLayoutDto dto)
    {
        try
        {
            var eventDto =
                await _eventService
                    .UpdateSeatLayoutAsync(
                        id,
                        dto);


            return Ok(eventDto);
        }
        catch (
            NotFoundException ex)
        {
            return NotFound(
                new
                {
                    message =
                        ex.Message
                });
        }
        catch (
            ValidationException ex)
        {
            return BadRequest(
                new
                {
                    message =
                        ex.Message
                });
        }
    }


    // DELETE /api/events/1
    [Authorize(
        Roles = "Administrator")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult>
        Delete(
            int id)
    {
        try
        {
            await _eventService
                .DeleteAsync(id);


            return Ok(
                new
                {
                    message =
                        "Event deleted successfully."
                });
        }
        catch (
            NotFoundException ex)
        {
            return NotFound(
                new
                {
                    message =
                        ex.Message
                });
        }
        catch (
            ConflictException ex)
        {
            return Conflict(
                new
                {
                    message =
                        ex.Message
                });
        }
    }
}