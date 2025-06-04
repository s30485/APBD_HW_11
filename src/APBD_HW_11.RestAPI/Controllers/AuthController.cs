using Microsoft.AspNetCore.Mvc;
using APBD_HW_11.RestAPI.Models;
using APBD_HW_11.RestAPI.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using APBD_HW_11.RestAPI.DTOs.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace APBD_HW_11.RestAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MasterContext _context;
    private readonly JwtHelper _jwtHelper;
    private readonly IPasswordHasher<Account> _passwordHasher;

    public AuthController(MasterContext context, JwtHelper jwtHelper, IPasswordHasher<Account> passwordHasher)
    {
        _context = context;
        _jwtHelper = jwtHelper;
        _passwordHasher = passwordHasher;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IResult> Authenticate([FromBody] LoginDto dto)
    {
        var account = await _context.Accounts
            .Include(a => a.Role)
            .FirstOrDefaultAsync(a => a.Username == dto.Username);

        if (account == null)
            return Results.Unauthorized();

        var result = _passwordHasher.VerifyHashedPassword(account, account.Password, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            return Results.Unauthorized();

        var token = _jwtHelper.GenerateToken(account);
        return Results.Ok(new TokenDto { AccessToken = token });
    }
}
