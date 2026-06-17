using BankApp.Models;
using System.Collections.Generic;
using System.Windows;

namespace BankApp.Views;

public partial class CreateInterestDialog : Window
{
    public int SelectedCardId => ((CardEntry)cbCard.SelectedItem).CardId;
    public int AmountCents { get; private set; }

    public CreateInterestDialog(List<CardModel> cards, BankModel? bank)
    {
        InitializeComponent();

        if (bank != null)
            tbRate.Text = $"Zinssatz: {bank.DisplayRate} p.a.";

        var entries = cards.ConvertAll(c => new CardEntry(c.Id, c.IBAN));
        cbCard.ItemsSource = entries;
        cbCard.SelectedIndex = 0;
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        errCard.Visibility   = Visibility.Collapsed;
        errAmount.Visibility = Visibility.Collapsed;

        bool valid = true;

        if (cbCard.SelectedItem == null)
        {
            errCard.Visibility = Visibility.Visible;
            valid = false;
        }

        if (!decimal.TryParse(tbAmount.Text.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out decimal euros) || euros < 1)
        {
            errAmount.Visibility = Visibility.Visible;
            valid = false;
        }

        if (!valid) return;

        AmountCents = (int)(euros * 100);
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private record CardEntry(int CardId, string Iban)
    {
        public string DisplayText => $"Karte #{CardId}  ·  {Iban}";
    }
}
