using BankApp.ViewModels;
using BankApp.Services;
using BankApp.Stores;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Commands
{
    public class NavigateCommand : CommandBase
    {
        private readonly NavigationService _navigationService;
        private readonly Func<ViewModelBase> _createViewModel;

        public NavigateCommand(NavigationService navigationService, Func<ViewModelBase> createViewModel)
        {
            _navigationService = navigationService;
            _createViewModel = createViewModel;
        }

        public override void Execute(object parameter)
        {
            _navigationService.Navigate(_createViewModel());
        }
    }
}
