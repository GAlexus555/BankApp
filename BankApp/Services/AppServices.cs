using BankApp.Services.Interfaces;
using BankApp.Stores;
using System;
using System.Net.Http;

namespace BankApp.Services;

public class AppServices
{
    public HttpClient HttpClient { get; }
    public ICardService CardService { get; }
    public IAccountService AccountService { get; }
    public ITransactionService TransactionService { get; }
    public NavigationService NavigationService { get; }

    public AppServices(string baseUrl, NavigationStore navigationStore)
    {
        HttpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        CardService = new CardService(HttpClient);
        AccountService = new AccountService(HttpClient, CardService);
        TransactionService = new TransactionService(HttpClient);
        NavigationService = new NavigationService(navigationStore);
    }
}
