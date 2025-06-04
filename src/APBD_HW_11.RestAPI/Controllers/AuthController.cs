using Microsoft.AspNetCore.Mvc;
using APBD_HW_11.RestAPI.Models;
using APBD_HW_11.RestAPI.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using APBD_HW_11.RestAPI.DTOs.Accounts;

namespace APBD_HW_11.RestAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MasterContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthController(MasterContext context, JwtHelper jwtHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
    }

    [HttpPost]
    public async Task<IResult> Authenticate(LoginDto dto)
    {
        var account = await _context.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Username == dto.Username);

        if (account == null)
            return Results.Unauthorized();

        using var hmac = new HMACSHA512(account.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

        if (!computedHash.SequenceEqual(account.PasswordHash))
            return Results.Unauthorized();

        var token = _jwtHelper.GenerateToken(account);
        return Results.Ok(new TokenDto { AccessToken = token });
    }
}