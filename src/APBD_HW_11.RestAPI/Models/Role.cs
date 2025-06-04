using System.ComponentModel.DataAnnotations;

namespace APBD_HW_11.RestAPI.Models;

public class Role
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;

    public ICollection<Account> Accounts { get; set; } = new List<Account>();
}
