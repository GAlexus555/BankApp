using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace BankApp.Views;

public partial class AddCardDialog : Window
{
    public string   IBAN         => Regex.Replace(tbIBAN.Text.Trim(), @"\s", "");
    public string   CardNr       => Regex.Replace(tbCardNr.Text.Trim(), @"\s", "");
    public int      CVC          => int.TryParse(tbCVC.Text.Trim(), out var v) ? v : 0;
    public DateTime ExpireDate   => dpExpireDate.SelectedDate ?? DateTime.Today.AddYears(3);
    public int      InitialCents => amountPicker.AmountInCents;

    public AddCardDialog()
    {
        InitializeComponent();
        dpExpireDate.SelectedDate = DateTime.Today.AddYears(3);
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        errIBAN.Visibility   = Visibility.Collapsed;
        errCardNr.Visibility = Visibility.Collapsed;
        errCVC.Visibility    = Visibility.Collapsed;
        errExpiry.Visibility = Visibility.Collapsed;

        bool valid = true;

        if (!IsValidIBAN(IBAN))
        {
            errIBAN.Visibility = Visibility.Visible;
            valid = false;
        }
        if (!IsValidCardNr(CardNr))
        {
            errCardNr.Visibility = Visibility.Visible;
            valid = false;
        }
        if (CVC < 100 || CVC > 9999)
        {
            errCVC.Visibility = Visibility.Visible;
            valid = false;
        }
        if (ExpireDate <= DateTime.Today)
        {
            errExpiry.Visibility = Visibility.Visible;
            valid = false;
        }

        if (valid) DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private static bool IsValidIBAN(string iban) =>
        iban.Length >= 15 && iban.Length <= 34 && Regex.IsMatch(iban, @"^[A-Z]{2}[A-Z0-9]+$", RegexOptions.IgnoreCase);

    private static bool IsValidCardNr(string cardNr) =>
        cardNr.Length >= 8 && cardNr.Length <= 19 && Regex.IsMatch(cardNr, @"^\d+$");
}
