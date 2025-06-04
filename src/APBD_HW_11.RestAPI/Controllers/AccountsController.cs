using Microsoft.AspNetCore.Mvc;
using APBD_HW_11.RestAPI.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using APBD_HW_11.RestAPI.DTOs.Accounts;

namespace APBD_HW_11.RestAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly MasterContext _context;

    public AccountsController(MasterContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IResult> Register(RegisterDto dto)
    {
        if (await _context.Accounts.AnyAsync(a => a.Username == dto.Username))
            return Results.BadRequest("Username already exists");

        using var hmac = new HMACSHA512();
        var account = new Account
        {
            Username = dto.Username,
            PasswordSalt = hmac.Key,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            RoleId = 2 //default to User
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return Results.Created($"/api/accounts/{account.Id}", new { account.Id, account.Username });
    }
}