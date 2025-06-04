using System.Text.Json;
using APBD_HW_09.RestAPI;
using APBD_HW_09.RestAPI.DTOs;
using APBD_HW_09.RestAPI.Interfaces;
using APBD_HW_09.RestAPI.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Default connection string not found");

builder.Services.AddDbContext<MasterContext>(options => options.UseSqlServer(ConnectionString));

builder.Services.AddSingleton<IValidatorService, ValidatorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/devices", async (MasterContext db, CancellationToken ct) =>
{
    try
    {
        var list = await db.Devices
            .Select(d => new DeviceDto(d.Id, d.Name))
            .ToListAsync(ct);
        return Results.Ok(list);
    }
    catch
    {
        return Results.Problem();
    }
});

app.MapGet("/api/devices/{id:int}", async (int id, MasterContext db, CancellationToken ct) =>
{
    var raw = await db.Devices
        .Include(d => d.DeviceType)
        .Include(d => d.DeviceEmployees)
        .ThenInclude(de => de.Employee)
        .ThenInclude(e => e.Person)
        .Where(d => d.Id == id)
        .Select(d => new
        {
            d.Name,
            DeviceTypeName = d.DeviceType.Name,
            d.IsEnabled,
            d.AdditionalProperties,
            Current = d.DeviceEmployees
                .Where(de => de.ReturnDate == null)
                .Select(de => new
                {
                    de.Employee.Id,
                    FullName = de.Employee.Person.FirstName + " " + de.Employee.Person.LastName
                })
                .FirstOrDefault()
        })
        .FirstOrDefaultAsync(ct);

    if (raw is null)
        return Results.NotFound();
    
    JsonElement props;
    try
    {
        using var doc = JsonDocument.Parse(raw.AdditionalProperties);
        props = doc.RootElement.Clone(); 
    }
    catch (JsonException)
    {
        // on invalid JSON, fallback to an empty object
        using var empty = JsonDocument.Parse("{}");
        props = empty.RootElement.Clone();
    }

    
    var dto = new DeviceDetailDto
    {
        Name = raw.Name,
        DeviceTypeName = raw.DeviceTypeName,
        IsEnabled = raw.IsEnabled,
        AdditionalProperties = props,
        CurrentEmployee = raw.Current is null
            ? null
            : new EmployeeRef(raw.Current.Id, raw.Current.FullName)
    };

    return Results.Ok(dto);
});

app.MapPost("/api/devices", async (CreateUpdateDeviceDto dto, MasterContext db, IValidatorService validator, CancellationToken ct) =>
{
    if (!validator.Validate(dto, out var errors))
        return Results.ValidationProblem(errors);

    var type = await db.DeviceTypes.SingleOrDefaultAsync(t => t.Name == dto.DeviceTypeName, ct);
    if (type == null)
        return Results.BadRequest(new { DeviceTypeName = $"'{dto.DeviceTypeName}' not found." });

    var device = new Device
    {
        Name = dto.Name,
        IsEnabled = dto.IsEnabled,
        DeviceTypeId = type.Id,
        AdditionalProperties = dto.AdditionalProperties.ToString()
    };

    db.Devices.Add(device);
    await db.SaveChangesAsync(ct);

    return Results.Created($"/api/devices/{device.Id}", new DeviceDto(device.Id, device.Name));
});

app.MapPut("/api/devices/{id:int}", async (int id, CreateUpdateDeviceDto dto, MasterContext db, IValidatorService validator, CancellationToken ct) =>
{
    if (!validator.Validate(dto, out var errors))
        return Results.ValidationProblem(errors);
    
    var device = await db.Devices.FindAsync(new object[] { id }, ct);
    if (device == null)
        return Results.NotFound();
    
    var type = await db.DeviceTypes
        .SingleOrDefaultAsync(t => t.Name == dto.DeviceTypeName, ct);
    if (type == null)
        return Results.BadRequest(new { DeviceTypeName = $"'{dto.DeviceTypeName}' not found." });
    
    device.Name = dto.Name;
    device.IsEnabled = dto.IsEnabled;
    device.DeviceTypeId = type.Id;
    device.AdditionalProperties = dto.AdditionalProperties.ToString();
    
    await db.SaveChangesAsync(ct);
    
    return Results.NoContent();
});

app.MapDelete("/api/devices/{id:int}", async (int id, MasterContext db, CancellationToken ct) =>
{
    var device = await db.Devices.FindAsync(new object[] { id }, ct);
    if (device == null)
        return Results.NotFound();

    db.Devices.Remove(device);
    await db.SaveChangesAsync(ct);
    return Results.NoContent();
});

app.MapGet("/api/employees", async (MasterContext db, CancellationToken ct) =>
{
    try
    {
        var list = await db.Employees
            .Include(e => e.Person)
            .Select(e => new EmployeeDto(e.Id,
                $"{e.Person.FirstName} {e.Person.LastName}"))
            .ToListAsync(ct);

        return Results.Ok(list);
    }
    catch
    {
        return Results.Problem();
    }
});

app.MapGet("/api/employees/{id:int}", async (int id, MasterContext db, CancellationToken ct) =>
{
    var emp = await db.Employees
        .Include(e => e.Person)
        .Include(e => e.Position)
        .Where(e => e.Id == id)
        .Select(e => new EmployeeDetailDto
        {
            PassportNumber = e.Person.PassportNumber,
            FirstName      = e.Person.FirstName,
            MiddleName     = e.Person.MiddleName,
            LastName       = e.Person.LastName,
            PhoneNumber    = e.Person.PhoneNumber,
            Email          = e.Person.Email,
            Salary         = e.Salary,
            Position       = new PositionRef(e.Position.Id, e.Position.Name),
            HireDate       = e.HireDate
        })
        .FirstOrDefaultAsync(ct);

    return emp is not null
        ? Results.Ok(emp)
        : Results.NotFound();
});

app.Run();