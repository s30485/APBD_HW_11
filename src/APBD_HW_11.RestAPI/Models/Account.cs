using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBD_HW_11.RestAPI.Models;

public class Account
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(3)]
    [RegularExpression("^[^0-9].*")]
    public string Username { get; set; } = null!;

    [Required]
    public byte[] PasswordHash { get; set; } = null!;

    [Required]
    public byte[] PasswordSalt { get; set; } = null!;

    [Required]
    [ForeignKey("Role")]
    public int RoleId { get; set; }

    public Role Role { get; set; } = null!;
}
