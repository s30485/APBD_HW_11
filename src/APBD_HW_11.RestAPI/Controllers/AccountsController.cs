using Microsoft.AspNetCore.Mvc;
using APBD_HW_11.RestAPI.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using APBD_HW_11.RestAPI.DTOs.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace APBD_HW_11.RestAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly MasterContext _context;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public AccountsController(MasterContext context, IPasswordHasher<Account> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<Account>> Register([FromBody] RegisterDto dto)
    {
        if (await _context.Accounts.AnyAsync(a => a.Username == dto.Username))
            return BadRequest("Username already exists");

        var account = new Account
        {
            Username = dto.Username,
            RoleId = 2 // default: User
        };

        account.Password = _passwordHasher.HashPassword(account, dto.Password);

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { id = account.Id }, new { account.Id, account.Username });
    }
}
