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
    public IInterestService InterestsService { get; }
    public IBankService BankService { get; }
    public IStatsService StatsService { get; }
    public NavigationService NavigationService { get; }

    public AppServices(string baseUrl, NavigationStore navigationStore)
    {
        HttpClient = new HttpClient(new UnauthorizedHandler { InnerHandler = new HttpClientHandler() })
        {
            BaseAddress = new Uri(baseUrl)
        };
        CardService = new CardService(HttpClient);
        AccountService = new AccountService(HttpClient, CardService);
        TransactionService = new TransactionService(HttpClient);
        InterestsService = new InterestService(HttpClient);
        BankService = new BankService(HttpClient);
        StatsService = new StatsService(HttpClient);
        NavigationService = new NavigationService(navigationStore);
    }
}
