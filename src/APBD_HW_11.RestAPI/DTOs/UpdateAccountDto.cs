namespace APBD_HW_11.RestAPI.DTOs;

public class UpdateAccountDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int? RoleId { get; set; }
}