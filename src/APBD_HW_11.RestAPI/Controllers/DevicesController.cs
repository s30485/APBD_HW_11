using APBD_HW_11.RestAPI.DTOs;
using APBD_HW_11.RestAPI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace APBD_HW_11.RestAPI.Controllers;

[ApiController]
[Route("api/devices")]
public class DevicesController : ControllerBase
{
    private readonly MasterContext _context;

    public DevicesController(MasterContext context)
    {
        _context = context;
    }

    // ADMIN: Get all devices
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetAll()
    {
        var devices = await _context.Devices
            .Select(d => new DeviceDto(d.Id, d.Name))
            .ToListAsync();
        return Results.Ok(devices);
    }

    // ADMIN & USER: Get specific device (restricted for user)
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IResult> GetDevice(int id)
    {
        var device = await _context.Devices
            .Include(d => d.DeviceType)
            .Include(d => d.DeviceEmployees)
            .ThenInclude(de => de.Employee)
            .ThenInclude(e => e.Person)
            .Where(d => d.Id == id)
            .Select(d => new
            {
                d.Id,
                d.Name,
                DeviceTypeName = d.DeviceType.Name,
                d.IsEnabled,
                d.AdditionalProperties,
                Current = d.DeviceEmployees
                    .Where(de => de.ReturnDate == null)
                    .Select(de => new
                    {
                        de.Employee.AccountId,
                        FullName = de.Employee.Person.FirstName + " " + de.Employee.Person.LastName
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        if (device == null) return Results.NotFound();

        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (userRole == "User" && device.Current?.AccountId != userId)
            return Results.Forbid();

        using var doc = JsonDocument.Parse(device.AdditionalProperties ?? "{}");

        return Results.Ok(new DeviceDetailDto
        {
            Name = device.Name,
            DeviceTypeName = device.DeviceTypeName,
            IsEnabled = device.IsEnabled,
            AdditionalProperties = doc.RootElement.Clone(),
            CurrentEmployee = device.Current is null
                ? null
                : new EmployeeRef(device.Id, device.Current.FullName)
        });
    }

    // ADMIN: Create
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> CreateDevice([FromBody] CreateUpdateDeviceDto dto)
    {
        var type = await _context.DeviceTypes.SingleOrDefaultAsync(t => t.Name == dto.DeviceTypeName);
        if (type == null)
            return Results.BadRequest(new { error = "Invalid device type" });

        var device = new Device
        {
            Name = dto.Name,
            IsEnabled = dto.IsEnabled,
            DeviceTypeId = type.Id,
            AdditionalProperties = dto.AdditionalProperties.ToString()
        };

        _context.Devices.Add(device);
        await _context.SaveChangesAsync();

        return Results.Created($"/api/devices/{device.Id}", new DeviceDto(device.Id, device.Name));
    }

    // ADMIN: Update
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> UpdateDevice(int id, [FromBody] CreateUpdateDeviceDto dto)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return Results.NotFound();

        var type = await _context.DeviceTypes.SingleOrDefaultAsync(t => t.Name == dto.DeviceTypeName);
        if (type == null)
            return Results.BadRequest(new { error = "Invalid device type" });

        device.Name = dto.Name;
        device.IsEnabled = dto.IsEnabled;
        device.DeviceTypeId = type.Id;
        device.AdditionalProperties = dto.AdditionalProperties.ToString();

        await _context.SaveChangesAsync();
        return Results.NoContent();
    }

    // ADMIN: Delete
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> DeleteDevice(int id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device == null) return Results.NotFound();

        _context.Devices.Remove(device);
        await _context.SaveChangesAsync();
        return Results.NoContent();
    }
}
