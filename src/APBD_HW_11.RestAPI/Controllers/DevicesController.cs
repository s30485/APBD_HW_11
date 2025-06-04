using APBD_HW_11.RestAPI.DTOs;
using APBD_HW_11.RestAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APBD_HW_11.RestAPI.Controllers
{
    [Route("api/devices")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> GetAllDevices()
        {
            return await _deviceService.GetAllDevicesAsync();
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IResult> GetDeviceById(int id)
        {
            return await _deviceService.GetDeviceByIdAsync(id, User);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> CreateDevice([FromBody] CreateUpdateDeviceDto dto)
        {
            return await _deviceService.CreateDeviceAsync(dto);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> UpdateDevice(int id, [FromBody] CreateUpdateDeviceDto dto)
        {
            return await _deviceService.UpdateDeviceAsync(id, dto);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IResult> DeleteDevice(int id)
        {
            return await _deviceService.DeleteDeviceAsync(id);
        }
    }
}