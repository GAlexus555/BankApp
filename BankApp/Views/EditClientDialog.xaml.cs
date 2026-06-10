using BankApp.Models;
using System.Text.RegularExpressions;
using System.Windows;

namespace BankApp.Views;

public partial class EditClientDialog : Window
{
    public string FirstName => tbFirstName.Text.Trim();
    public string LastName  => tbLastName.Text.Trim();
    public string Email     => tbEmail.Text.Trim();
    public string Phone     => tbPhone.Text.Trim();
    public string Address   => tbAddress.Text.Trim();

    public EditClientDialog(AccountModel client)
    {
        InitializeComponent();
        tbFirstName.Text = client.FirstName;
        tbLastName.Text  = client.LastName;
        tbEmail.Text     = client.Email;
        tbPhone.Text     = client.Phone;
        tbAddress.Text   = client.Address;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        errFirstName.Visibility = Visibility.Collapsed;
        errLastName.Visibility  = Visibility.Collapsed;
        errEmail.Visibility     = Visibility.Collapsed;
        errPhone.Visibility     = Visibility.Collapsed;
        errAddress.Visibility   = Visibility.Collapsed;

        bool valid = true;
        if (FirstName.Length < 2)    { errFirstName.Visibility = Visibility.Visible; valid = false; }
        if (LastName.Length < 2)     { errLastName.Visibility  = Visibility.Visible; valid = false; }
        if (!IsValidEmail(Email))    { errEmail.Visibility     = Visibility.Visible; valid = false; }
        if (!HasMinDigits(Phone, 7)) { errPhone.Visibility     = Visibility.Visible; valid = false; }
        if (Address.Length < 3)      { errAddress.Visibility   = Visibility.Visible; valid = false; }

        if (valid) DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");

    private static bool HasMinDigits(string text, int min) =>
        Regex.Matches(text, @"\d").Count >= min;
}
