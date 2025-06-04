using System.Security.Claims;
using APBD_HW_11.RestAPI.DTOs;
using APBD_HW_11.RestAPI.DTOs.Accounts;

namespace APBD_HW_11.RestAPI.Interfaces;

public interface IAccountService
{
    Task<IResult> GetAllAccountsAsync();
    Task<IResult> GetAccountByIdAsync(int id);
    Task<IResult> GetOwnAccountAsync(ClaimsPrincipal user);
    Task<IResult> CreateAccountAsync(RegisterDto dto);
    Task<IResult> UpdateOwnAccountAsync(ClaimsPrincipal user, UpdateAccountDto dto);
    Task<IResult> DeleteAccountAsync(int id);
}
