namespace APBD_HW_09.RestAPI.DTOs;

public record EmployeeDetailDto
{
    public string PassportNumber { get; init; } = default!;
    public string FirstName      { get; init; } = default!;
    public string? MiddleName    { get; init; }
    public string LastName       { get; init; } = default!;
    public string PhoneNumber    { get; init; } = default!;
    public string Email          { get; init; } = default!;
    public decimal Salary        { get; init; }
    public PositionRef Position  { get; init; } = default!;
    public DateTime HireDate     { get; init; }
}