using APBD_HW_11.RestAPI.DTOs;
using APBD_HW_11.RestAPI.Models;
using APBD_HW_11.RestAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APBD_HW_11.RestAPI.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly MasterContext _context;

        public DeviceService(MasterContext context)
        {
            _context = context;
        }

        public async Task<IResult> GetAllDevicesAsync()
        {
            var devices = await _context.Devices
                .Select(d => new DeviceDto(d.Id, d.Name))
                .ToListAsync();

            return Results.Ok(devices);
        }

        public async Task<IResult> GetDeviceByIdAsync(int id, ClaimsPrincipal user)
        {
            var isAdmin = user.IsInRole("Admin");
            var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var device = await _context.Devices
                .Include(d => d.DeviceType)
                .Include(d => d.DeviceEmployees)
                .ThenInclude(de => de.Employee)
                .ThenInclude(e => e.Person)
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();

            if (device == null)
                return Results.NotFound();

            if (!isAdmin && device.DeviceEmployees.All(de => de.Employee.PersonId.ToString() != userIdClaim))
                return Results.Forbid();

            return Results.Ok(new DeviceDetailDto
            {
                Name = device.Name,
                DeviceTypeName = device.DeviceType.Name,
                IsEnabled = device.IsEnabled,
                AdditionalProperties = System.Text.Json.JsonDocument.Parse(device.AdditionalProperties).RootElement,
                CurrentEmployee = device.DeviceEmployees
                    .Where(de => de.ReturnDate == null)
                    .Select(de => new EmployeeRef(de.Employee.Id, $"{de.Employee.Person.FirstName} {de.Employee.Person.LastName}"))
                    .FirstOrDefault()
            });
        }

        public async Task<IResult> CreateDeviceAsync(CreateUpdateDeviceDto dto)
        {
            var type = await _context.DeviceTypes.SingleOrDefaultAsync(t => t.Name == dto.DeviceTypeName);
            if (type == null)
                return Results.BadRequest(new { DeviceTypeName = $"'{dto.DeviceTypeName}' not found." });

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

        public async Task<IResult> UpdateDeviceAsync(int id, CreateUpdateDeviceDto dto)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
                return Results.NotFound();

            var type = await _context.DeviceTypes.SingleOrDefaultAsync(t => t.Name == dto.DeviceTypeName);
            if (type == null)
                return Results.BadRequest(new { DeviceTypeName = $"'{dto.DeviceTypeName}' not found." });

            device.Name = dto.Name;
            device.IsEnabled = dto.IsEnabled;
            device.DeviceTypeId = type.Id;
            device.AdditionalProperties = dto.AdditionalProperties.ToString();

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }

        public async Task<IResult> DeleteDeviceAsync(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device == null)
                return Results.NotFound();

            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
