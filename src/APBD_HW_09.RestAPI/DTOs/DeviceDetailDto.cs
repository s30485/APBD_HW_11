using System.Text.Json;

namespace APBD_HW_09.RestAPI.DTOs;

public record DeviceDetailDto
{
    public string Name { get; init; } = default!;
    public string DeviceTypeName { get; init; } = default!;
    public bool IsEnabled { get; init; }
    public JsonElement AdditionalProperties { get; init; }
    public EmployeeRef? CurrentEmployee { get; init; }
}