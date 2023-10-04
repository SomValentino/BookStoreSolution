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
    private readonly ILogger<ITokenAccountRepository> _logger;

    private string UserId => User.Claims.FirstOrDefault (_ => _.Type == JwtClaimTypes.Subject)?.Value ?? string.Empty;

    public TokenAccountController (ITokenAccountRepository tokenAccountRepository,
        ILogger<ITokenAccountRepository> logger) {
        _tokenAccountRepository = tokenAccountRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetBalance () {

        _logger.LogInformation ("Getting Balance for userId: {id}", UserId);
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();

        var tokenAccount = await GetTokenAccountAsync ();

        _logger.LogInformation ("Sucessfully fetched token balance");

        return Ok (new { Balance = tokenAccount.BookPurchaseToken });
    }

    [HttpPost ("deposit")]
    public async Task<IActionResult> Deposit ([FromQuery] long amount) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var tokenAccount = await GetTokenAccountAsync ();

        _logger.LogInformation ("Depositing token for userId: {id} with amount {amount}", UserId, amount);

        tokenAccount.Deposit (amount);

        await _tokenAccountRepository.UpdateTokenAccount (tokenAccount);

        _logger.LogInformation ("Sucessfully deposited token balance");

        return NoContent ();
    }

    [HttpPost ("withdraw")]
    public async Task<IActionResult> WithDraw ([FromQuery] long amount) {
        if (string.IsNullOrEmpty (UserId)) return Unauthorized ();
        var tokenAccount = await GetTokenAccountAsync ();

        _logger.LogInformation ("Withdrawing token of amount {amount} for userId: {id}", UserId, amount);

        if (amount > tokenAccount.BookPurchaseToken) return BadRequest ("Insufficient funds");

        tokenAccount.WithDraw (amount);

        await _tokenAccountRepository.UpdateTokenAccount (tokenAccount);

        _logger.LogInformation ("Sucessfully withdrawn token balance");

        return NoContent ();
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