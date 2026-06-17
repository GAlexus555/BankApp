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
        // Use InvariantCulture so decimal separator is always '.' in format strings
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

    // ── BankModel ──────────────────────────────────────────────────────────────

    [Fact]
    public void BankModel_DisplayRate_FormatsAsPercent()
    {
        var bank = new BankModel { InterestRate = 0.035 };
        Assert.Equal("3.50 %", bank.DisplayRate);
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
}
