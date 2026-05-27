using System;
using System.Collections.Generic;
using System.Text;
using BankApp.Services;

namespace BankApp.ViewModels;

public class AccountViewModel : ViewModelBase
{
    //Navigation
    private NavigationService _navigationService;

    public AccountViewModel(NavigationService navigationService)
    {
        _navigationService = navigationService;
    }
}
