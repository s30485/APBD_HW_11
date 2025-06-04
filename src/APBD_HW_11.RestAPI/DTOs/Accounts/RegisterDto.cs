using System.ComponentModel.DataAnnotations;

namespace APBD_HW_11.RestAPI.DTOs.Accounts;

public class RegisterDto
{
    [Required]
    [MinLength(3)]
    [RegularExpression("^[^0-9].*")]
    public string Username { get; set; } = null!;

    [Required]
    [MinLength(12)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$")]
    public string Password { get; set; } = null!;
    
    [Required]
    public int? EmployeeId { get; set; } = null!;
    
    public int? RoleId { get; set; } = null!;
}
