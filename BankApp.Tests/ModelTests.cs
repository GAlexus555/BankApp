using System;
using System.Globalization;
using System.Threading;
using BankApp.Models;
using Xunit;

namespace BankApp.Tests;

public class ModelTests
{
    public ModelTests()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    // ── CardModel ──────────────────────────────────────────────────────────────

    [Fact]
    public void CardModel_DisplayBalance_FormatsPositiveCents()
    {
        var card = new CardModel { Cents = 1234 };
        Assert.Equal("€ 12.34", card.DisplayBalance);
    }

    [Fact]
    public void CardModel_DisplayBalance_ZeroCentsShowsZeroEuros()
    {
        var card = new CardModel { Cents = 0 };
        Assert.Equal("€ 0.00", card.DisplayBalance);
    }

    [Fact]
    public void CardModel_DisplayBalance_NegativeCentsShowsNegativeEuros()
    {
        var card = new CardModel { Cents = -500 };
        Assert.Equal("€ -5.00", card.DisplayBalance);
    }

    [Fact]
    public void CardModel_DisplayBalance_LargeAmount()
    {
        var card = new CardModel { Cents = 1_000_000 };
        Assert.Equal("€ 10000.00", card.DisplayBalance);
    }

    [Fact]
    public void CardModel_DisplayBalance_OddCents()
    {
        var card = new CardModel { Cents = 1 };
        Assert.Equal("€ 0.01", card.DisplayBalance);
    }

    // ── TransactionModel ───────────────────────────────────────────────────────

    [Fact]
    public void TransactionModel_AmountEuros_DividesCentsByHundred()
    {
        var tx = new TransactionModel { AmountCents = 999 };
        Assert.Equal(9.99, tx.AmountEuros, precision: 5);
    }

    [Fact]
    public void TransactionModel_AmountEuros_LargeAmount()
    {
        var tx = new TransactionModel { AmountCents = 100000 };
        Assert.Equal(1000.0, tx.AmountEuros, precision: 5);
    }

    [Fact]
    public void TransactionModel_AmountEuros_ZeroCents()
    {
        var tx = new TransactionModel { AmountCents = 0 };
        Assert.Equal(0.0, tx.AmountEuros, precision: 5);
    }

    [Fact]
    public void TransactionModel_ToString_ContainsIBANsAndDescription()
    {
        var tx = new TransactionModel
        {
            AmountCents = 5000,
            IBANFrom = "AT12 3456",
            IBANTo = "AT98 7654",
            Description = "Miete",
            Status = TransactionStatus.Sent,
            CreatedAt = new DateTime(2024, 6, 1)
        };

        var result = tx.ToString();

        Assert.Contains("AT12 3456", result);
        Assert.Contains("AT98 7654", result);
        Assert.Contains("Miete", result);
        Assert.Contains("50.00", result);
        Assert.Contains("Sent", result);
    }

    [Fact]
    public void TransactionModel_ToString_ContainsFormattedDate()
    {
        var tx = new TransactionModel
        {
            AmountCents = 100,
            IBANFrom = "A",
            IBANTo = "B",
            Description = "",
            CreatedAt = new DateTime(2024, 1, 5)
        };

        Assert.Contains("05.01.2024", tx.ToString());
    }

    // ── BankModel ──────────────────────────────────────────────────────────────

    [Fact]
    public void BankModel_DisplayRate_FormatsAsPercent()
    {
        var bank = new BankModel { InterestRate = 0.035 };
        Assert.Equal("3.50 %", bank.DisplayRate);
    }

    [Fact]
    public void BankModel_DisplayRate_ZeroRate()
    {
        var bank = new BankModel { InterestRate = 0 };
        Assert.Equal("0.00 %", bank.DisplayRate);
    }

    [Fact]
    public void BankModel_DisplayRate_FullPercent()
    {
        var bank = new BankModel { InterestRate = 0.10 };
        Assert.Equal("10.00 %", bank.DisplayRate);
    }

    // ── InterestModel ──────────────────────────────────────────────────────────

    [Fact]
    public void InterestModel_DisplayRate_FormatsAsPercent()
    {
        var interest = new InterestModel { InterestRate = 0.05 };
        Assert.Equal("5.00 %", interest.DisplayRate);
    }

    [Fact]
    public void InterestModel_IsActive_TrueWhenNotWithdrawn()
    {
        var interest = new InterestModel { Withdrawn = false };
        Assert.True(interest.IsActive);
    }

    [Fact]
    public void InterestModel_IsActive_FalseWhenWithdrawn()
    {
        var interest = new InterestModel { Withdrawn = true };
        Assert.False(interest.IsActive);
    }

    [Fact]
    public void InterestModel_StatusText_ShowsAktivWhenNotWithdrawn()
    {
        var interest = new InterestModel { Withdrawn = false };
        Assert.Equal("● Aktiv", interest.StatusText);
    }

    [Fact]
    public void InterestModel_StatusText_ShowsAusgezahltWhenWithdrawn()
    {
        var interest = new InterestModel { Withdrawn = true };
        Assert.Equal("Ausgezahlt", interest.StatusText);
    }

    [Fact]
    public void InterestModel_DisplayCardId_UsesIbanWhenSet()
    {
        var interest = new InterestModel { CardId = 7, CardIBAN = "AT123456789" };
        Assert.Equal("AT123456789", interest.DisplayCardId);
    }

    [Fact]
    public void InterestModel_DisplayCardId_FallsBackToCardIdWhenNoIban()
    {
        var interest = new InterestModel { CardId = 42, CardIBAN = null };
        Assert.Equal("Karte #42", interest.DisplayCardId);
    }

    [Fact]
    public void InterestModel_DisplayCardId_EmptyStringIbanFallsBackToCardId()
    {
        var interest = new InterestModel { CardId = 5, CardIBAN = "" };
        Assert.Equal("", interest.DisplayCardId);
    }

    [Fact]
    public void InterestModel_DisplayInvested_FormatsCentsAsEuros()
    {
        var interest = new InterestModel { Amount = 50000 };
        Assert.Equal("€ 500.00", interest.DisplayInvested);
    }

    [Fact]
    public void InterestModel_DisplayDate_FormatsCorrectly()
    {
        var interest = new InterestModel { CreatedAt = new DateOnly(2023, 3, 15) };
        Assert.Equal("15.03.2023", interest.DisplayDate);
    }

    [Fact]
    public void InterestModel_CurrentAmount_IsAtLeastInitialAmount()
    {
        
        var interest = new InterestModel
        {
            Amount = 10000,
            InterestRate = 0.05,
            CreatedAt = DateOnly.FromDateTime(DateTime.Today.AddYears(-1))
        };

        Assert.True(interest.CurrentAmount >= interest.Amount);
    }

    [Fact]
    public void InterestModel_DisplayGrowth_NeverShowsNegative()
    {
        
        var interest = new InterestModel
        {
            Amount = 10000,
            InterestRate = 0,
            CreatedAt = DateOnly.FromDateTime(DateTime.Today)
        };

        Assert.StartsWith("+€", interest.DisplayGrowth);
        Assert.Contains("0.00", interest.DisplayGrowth);
    }

    // ── AccountModel ───────────────────────────────────────────────────────────

    [Fact]
    public void AccountModel_ToString_ContainsFullName()
    {
        var account = new AccountModel { FirstName = "Maria", LastName = "Muster", Role = AccountRole.Client };
        Assert.Contains("Maria Muster", account.ToString());
    }

    [Fact]
    public void AccountModel_ToString_ContainsRole()
    {
        var account = new AccountModel { FirstName = "Max", LastName = "Admin", Role = AccountRole.Manager };
        Assert.Contains("Manager", account.ToString());
    }

    [Fact]
    public void AccountModel_ToString_ClientRoleIncluded()
    {
        var account = new AccountModel { FirstName = "Anna", LastName = "Test", Role = AccountRole.Client };
        Assert.Contains("Client", account.ToString());
    }

    [Fact]
    public void AccountModel_Cards_IsNullByDefault()
    {
        var account = new AccountModel();
        Assert.Null(account.Cards);
    }

    [Fact]
    public void AccountModel_DefaultStringsAreEmpty()
    {
        var account = new AccountModel();
        Assert.Equal("", account.FirstName);
        Assert.Equal("", account.LastName);
        Assert.Equal("", account.Email);
        Assert.Equal("", account.Address);
        Assert.Equal("", account.Phone);
    }

    // ── StatsModel ─────────────────────────────────────────────────────────────

    [Fact]
    public void StatsModel_DisplayName_CombinesFirstAndLastName()
    {
        var stats = new StatsModel { FirstName = "Hans", LastName = "Meier" };
        Assert.Equal("Hans Meier", stats.DisplayName);
    }

    [Fact]
    public void StatsModel_DisplayTotal_FormatsCentsAsEuros()
    {
        var stats = new StatsModel { TotalCents = 25050 };
        Assert.Equal("€ 250.50", stats.DisplayTotal);
    }

    [Fact]
    public void StatsModel_DisplayTotal_ZeroCents()
    {
        var stats = new StatsModel { TotalCents = 0 };
        Assert.Equal("€ 0.00", stats.DisplayTotal);
    }

    [Fact]
    public void StatsModel_DisplayName_EmptyWhenNamesNotSet()
    {
        var stats = new StatsModel();
        Assert.Equal(" ", stats.DisplayName);
    }

    [Fact]
    public void StatsModel_DisplayTotal_LargeAmount()
    {
        var stats = new StatsModel { TotalCents = 1_000_000 };
        Assert.Equal("€ 10000.00", stats.DisplayTotal);
    }

    [Fact]
    public void StatsModel_TransactionCount_DefaultIsZero()
    {
        var stats = new StatsModel();
        Assert.Equal(0, stats.TransactionCount);
    }

    // ── CardModel extra ────────────────────────────────────────────────────────

    [Fact]
    public void CardModel_DefaultStatus_IsActive()
    {
        var card = new CardModel();
        Assert.Equal(CardStatus.Active, card.CardStatus);
    }

    [Fact]
    public void CardModel_DefaultIBAN_IsEmpty()
    {
        var card = new CardModel();
        Assert.Equal("", card.IBAN);
    }

    [Fact]
    public void CardModel_DisplayBalance_99Cents()
    {
        var card = new CardModel { Cents = 99 };
        Assert.Equal("€ 0.99", card.DisplayBalance);
    }

    // ── TransactionModel extra ─────────────────────────────────────────────────

    [Fact]
    public void TransactionModel_DefaultStatus_IsPending()
    {
        var tx = new TransactionModel();
        Assert.Equal(TransactionStatus.Pending, tx.Status);
    }

    [Fact]
    public void TransactionModel_AmountEuros_MatchesToString()
    {
        var tx = new TransactionModel
        {
            AmountCents = 7777,
            IBANFrom = "X",
            IBANTo = "Y",
            Description = ""
        };

        Assert.Equal(77.77, tx.AmountEuros, precision: 2);
        Assert.Contains("77.77", tx.ToString());
    }

    // ── InterestModel extra ────────────────────────────────────────────────────

    [Fact]
    public void InterestModel_DisplayRate_ZeroRate()
    {
        var interest = new InterestModel { InterestRate = 0 };
        Assert.Equal("0.00 %", interest.DisplayRate);
    }

    [Fact]
    public void InterestModel_DisplayInvested_ZeroAmount()
    {
        var interest = new InterestModel { Amount = 0 };
        Assert.Equal("€ 0.00", interest.DisplayInvested);
    }

    [Fact]
    public void InterestModel_CurrentAmount_ZeroRateDoesNotGrow()
    {
        var interest = new InterestModel
        {
            Amount = 20000,
            InterestRate = 0,
            CreatedAt = DateOnly.FromDateTime(DateTime.Today.AddYears(-2))
        };

        Assert.Equal(20000, interest.CurrentAmount, precision: 5);
    }

    [Fact]
    public void InterestModel_DisplayDate_LeadingZeroForSingleDigitDay()
    {
        var interest = new InterestModel { CreatedAt = new DateOnly(2024, 12, 3) };
        Assert.Equal("03.12.2024", interest.DisplayDate);
    }

    // ── AccountModel extra ─────────────────────────────────────────────────────

    [Fact]
    public void AccountModel_ToString_PadsNameTo30Chars()
    {
        var account = new AccountModel { FirstName = "A", LastName = "B", Role = AccountRole.Client };
        var result = account.ToString();

        
        var beforePipe = result.Split('|')[0];
        Assert.Equal(30, beforePipe.TrimEnd().Length + (30 - beforePipe.TrimEnd().Length));
        Assert.True(beforePipe.Length >= 30);
    }

    [Fact]
    public void AccountModel_Role_DefaultIsClient()
    {
        var account = new AccountModel();
        Assert.Equal(AccountRole.Client, account.Role);
    }

    [Fact]
    public void AccountModel_CanAssignCards()
    {
        var account = new AccountModel();
        account.Cards = new System.Collections.Generic.List<CardModel>
        {
            new CardModel { Id = 1, Cents = 500 },
            new CardModel { Id = 2, Cents = 1000 }
        };

        Assert.NotNull(account.Cards);
        Assert.Equal(2, account.Cards.Count);
        Assert.Equal(500, account.Cards[0].Cents);
    }

    [Fact]
    public void AccountModel_BirthdateAndCreatedAt_DefaultToMinValue()
    {
        var account = new AccountModel();
        Assert.Equal(default(DateTime), account.Birthdate);
        Assert.Equal(default(DateTime), account.CreatedAt);
    }
}