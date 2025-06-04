using System.ComponentModel.DataAnnotations;

namespace APBD_HW_11.RestAPI.DTOs.Accounts;

public class LoginDto
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
