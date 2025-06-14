﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace APBD_HW_11.RestAPI.DTOs;

public record CreateUpdateDeviceDto
{
    [Required, StringLength(150)]
    public string Name { get; init; } = default!;

    [Required]
    public string DeviceTypeName { get; init; } = default!;

    public bool IsEnabled { get; init; } = true;

    [Required]
    public JsonElement AdditionalProperties { get; init; }
}