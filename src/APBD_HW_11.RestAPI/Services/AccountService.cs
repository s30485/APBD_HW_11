using System.Security.Claims;
using APBD_HW_11.RestAPI.DTOs;
using APBD_HW_11.RestAPI.DTOs.Accounts;
using APBD_HW_11.RestAPI.Interfaces;
using APBD_HW_11.RestAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace APBD_HW_11.RestAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly MasterContext _context;
        private readonly IPasswordHasher<Account> _hasher;

        public AccountService(MasterContext context, IPasswordHasher<Account> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task<IResult> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts
                .Select(a => new { a.Id, a.Username, a.Password })
                .ToListAsync();

            return Results.Ok(accounts);
        }

        public async Task<IResult> GetAccountByIdAsync(int id)
        {
            var acc = await _context.Accounts.FindAsync(id);
            return acc is not null
                ? Results.Ok(new { acc.Username, acc.Password })
                : Results.NotFound();
        }

        public async Task<IResult> GetOwnAccountAsync(ClaimsPrincipal user)
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var acc = await _context.Accounts.FindAsync(userId);
            return acc is not null
                ? Results.Ok(new { acc.Username })
                : Results.NotFound();
        }

        public async Task<IResult> CreateAccountAsync(RegisterDto dto)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role == null) return Results.BadRequest("Default role not found");

            var acc = new Account
            {
                Username = dto.Username,
                EmployeeId = dto.EmployeeId,
                RoleId = role.Id
            };

            acc.Password = _hasher.HashPassword(acc, dto.Password);
            _context.Accounts.Add(acc);
            await _context.SaveChangesAsync();
            return Results.Created($"/api/accounts/{acc.Id}", new { acc.Id });
        }

        public async Task<IResult> UpdateOwnAccountAsync(ClaimsPrincipal user, UpdateAccountDto dto)
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var acc = await _context.Accounts.FindAsync(userId);
            if (acc == null) return Results.NotFound();

            acc.Username = dto.Username ?? acc.Username;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                acc.Password = _hasher.HashPassword(acc, dto.Password);

            await _context.SaveChangesAsync();
            return Results.Ok();
        }

        public async Task<IResult> DeleteAccountAsync(int id)
        {
            var acc = await _context.Accounts.FindAsync(id);
            if (acc == null) return Results.NotFound();
            _context.Accounts.Remove(acc);
            await _context.SaveChangesAsync();
            return Results.NoContent();
        }
    }
}
