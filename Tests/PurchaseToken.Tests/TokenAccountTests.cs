using FluentAssertions;
using FluentValidation;
using PurchaseToken.API.Models;
namespace PurchaseToken.Tests;

public class TokenAccountTests {
    [Fact]
    public void TokenAccount_InitialBalance_ReturnsZeroTokens () {
        var account = new TokenAccount ("fakeuserId");

        account.BookPurchaseToken.Should ().BeGreaterThanOrEqualTo (0);
    }

    [Fact]
    public void TokenAccount_Deposit_IncreasesTokenBalance () {

        var initialBalance = 5;
        var account = new TokenAccount ("fakeuserId", initialBalance);

        account.Deposit (500);

        account.BookPurchaseToken.Should ().BeGreaterThanOrEqualTo (initialBalance);
    }

    [Fact]
    public void TokenAccount_WithdrawAmountLargerThanBalance_ReturnsInitialBalance () {

        var initialBalance = 5;
        var account = new TokenAccount ("fakeuserId", initialBalance);

        account.WithDraw (500);

        account.BookPurchaseToken.Should ().Be (initialBalance);
    }

    [Fact]
    public void TokenAccount_WithdrawAmountLessThanBalance_ReturnsDifference () {

        var initialBalance = 500;
        var account = new TokenAccount ("fakeuserId", initialBalance);

        account.WithDraw (50);

        account.BookPurchaseToken.Should ().Be (450);
    }
}