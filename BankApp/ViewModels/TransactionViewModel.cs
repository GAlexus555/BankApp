using BankApp.Commands;
using BankApp.Models;
using BankApp.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Input;

namespace BankApp.ViewModels;

public class TransactionViewModel : ViewModelBase
{
    private readonly AppServices _services;
    private readonly CardViewModel _card;
    private readonly AccountModel _account;

    public string IbanFrom => _card.IBAN;

    private string _ibanTo = "";
    public string IbanTo
    {
        get => _ibanTo;
        set { _ibanTo = value; OnPropertyChanged(nameof(IbanTo)); ValidationError = null; }
    }

    private string _description = "";
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(nameof(Description)); ValidationError = null; }
    }

    public int Amount { get; set; }

    private string? _validationError;
    public string? ValidationError
    {
        get => _validationError;
        private set
        {
            _validationError = value;
            OnPropertyChanged(nameof(ValidationError));
            OnPropertyChanged(nameof(HasValidationError));
        }
    }
    public bool HasValidationError => !string.IsNullOrEmpty(_validationError);

    public IRelayCommand NewTransaction { get; }
    public ICommand Back { get; }

    public TransactionViewModel(AppServices services, CardViewModel card, AccountModel account)
    {
        _services = services;
        _card     = card;
        _account  = account;

        Back           = new NavigateCommand(services.NavigationService, () => new AccountViewModel(services, account));
        NewTransaction = new AsyncRelayCommand(CreateTransaction);
    }

    private async Task CreateTransaction()
    {
        var iban = IbanTo.Trim();
        if (string.IsNullOrWhiteSpace(iban) || iban.Length < 15)
        {
            ValidationError = "Bitte gültige Empfänger-IBAN eingeben (min. 15 Zeichen).";
            return;
        }
        if (Amount <= 0)
        {
            ValidationError = "Betrag muss größer als 0 sein.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Description))
        {
            ValidationError = "Bitte Verwendungszweck angeben.";
            return;
        }

        ValidationError = null;

        var success = await _services.TransactionService.CreateNewTransaction(
            IbanFrom, iban, Amount, Description, TransactionStatus.Pending);

        if (success == null)
        {
            MessageBox.Show("Überweisung fehlgeschlagen. Bitte Eingaben prüfen.");
            return;
        }

        var account = await _services.AccountService.GetMeAsync();
        if (account == null)
        {
            MessageBox.Show("Kontodaten konnten nicht geladen werden.");
            return;
        }

        MessageBox.Show("Überweisung erfolgreich durchgeführt.");
        _services.NavigationService.Navigate(new AccountViewModel(_services, account));
    }
}
