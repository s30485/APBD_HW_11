using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using APBD_HW_11.RestAPI;
using System.Security.Cryptography;
using System.Text;
using APBD_HW_11.RestAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using APBD_HW_11.RestAPI.DTOs.Accounts;
using APBD_HW_11.RestAPI.Interfaces;
using APBD_HW_11.RestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace APBD_HW_11.RestAPI.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetAllAccounts()
        => await _accountService.GetAllAccountsAsync();

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> GetAccountById(int id)
        => await _accountService.GetAccountByIdAsync(id);

    [HttpGet("me")]
    [Authorize(Roles = "User")]
    public async Task<IResult> GetOwnAccount()
        => await _accountService.GetOwnAccountAsync(User);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> CreateAccount(RegisterDto dto)
        => await _accountService.CreateAccountAsync(dto);

    [HttpPut("me")]
    [Authorize(Roles = "User")]
    public async Task<IResult> UpdateOwnAccount(UpdateAccountDto dto)
        => await _accountService.UpdateOwnAccountAsync(User, dto);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IResult> DeleteAccount(int id)
        => await _accountService.DeleteAccountAsync(id);
}
