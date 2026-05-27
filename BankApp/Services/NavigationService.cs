using BankApp.ViewModels;
using BankApp.Stores;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Services;

public class NavigationService
{
    private readonly NavigationStore _navigationStore;

    public NavigationService(NavigationStore navigationStore)
    {
        _navigationStore = navigationStore;
        Log.Information($"Navigation service has been created");
    }

    public void Navigate(ViewModelBase viewModel)
    {
        _navigationStore.CurrentViewModel = viewModel;
        Log.Information($"Navigating to {viewModel.ToString()}...");
    }
}
