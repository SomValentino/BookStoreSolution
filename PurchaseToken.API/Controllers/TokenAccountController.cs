using System.Runtime.CompilerServices;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseToken.API.Data.Repository.Interfaces;
using PurchaseToken.API.Models;

namespace PurchaseToken.API.Controllers;

[Authorize]
[ApiController]
[Route ("[controller]")]
public class TokenAccountController : ControllerBase {
    private readonly ITokenAccountRepository _tokenAccountRepository;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject) !.Value;

    public TokenAccountController (ITokenAccountRepository tokenAccountRepository) {
        _tokenAccountRepository = tokenAccountRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetBalance () {

        var tokenAccount = await GetTokenAccountAsync ();

        return Ok (new { Balance = tokenAccount.BookPurchaseToken });
    }

    [HttpPost ("deposit")]
    public async Task<IActionResult> Deposit ([FromQuery] long amount) {
        var tokenAccount = await GetTokenAccountAsync ();

        tokenAccount.Deposit (amount);

        await _tokenAccountRepository.UpdateTokenAccount (tokenAccount);

        return NoContent ();
    }

    [HttpPost ("withdraw")]
    public async Task<IActionResult> WithDraw ([FromQuery] long amount) {
        var tokenAccount = await GetTokenAccountAsync ();

        if(amount > tokenAccount.BookPurchaseToken) return BadRequest("Insufficient funds");

        tokenAccount.WithDraw(amount);

        await _tokenAccountRepository.UpdateTokenAccount(tokenAccount);

        return NoContent();
    }

    #region private methods

    private async Task<TokenAccount> GetTokenAccountAsync () {
        var tokenAccount = await _tokenAccountRepository.GetUserBalance (UserId);

        if (tokenAccount == null) {
            tokenAccount = new TokenAccount (UserId, 0);
            await _tokenAccountRepository.CreateTokenAccount (tokenAccount);
        }
        return tokenAccount;
    }
    #endregion
}