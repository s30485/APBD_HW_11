using APBD_HW_11.RestAPI.DTOs;
using System.Security.Claims;

namespace APBD_HW_11.RestAPI.Interfaces
{
    public interface IDeviceService
    {
        Task<IResult> GetAllDevicesAsync();
        Task<IResult> GetDeviceByIdAsync(int id, ClaimsPrincipal user);
        Task<IResult> CreateDeviceAsync(CreateUpdateDeviceDto dto);
        Task<IResult> UpdateDeviceAsync(int id, CreateUpdateDeviceDto dto);
        Task<IResult> DeleteDeviceAsync(int id);
    }
}