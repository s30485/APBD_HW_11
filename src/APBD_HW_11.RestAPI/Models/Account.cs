using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBD_HW_11.RestAPI.Models;

public class Account
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    public int RoleId { get; set; }
    
    public int? EmployeeId { get; set; }
    public Role Role { get; set; } = null!;
    
    public Employee Employee { get; set; } = null!;
}

