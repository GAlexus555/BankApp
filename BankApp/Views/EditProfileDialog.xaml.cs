using BankApp.Models;
using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace BankApp.Views;

public partial class EditProfileDialog : Window
{
    public string   FirstName => tbFirstName.Text.Trim();
    public string   LastName  => tbLastName.Text.Trim();
    public string   Email     => tbEmail.Text.Trim();
    public string   Phone     => tbPhone.Text.Trim();
    public string   Address   => tbAddress.Text.Trim();
    public DateTime Birthdate => dpBirthdate.SelectedDate ?? DateTime.Today.AddYears(-18);
    public string   Password  => pbPassword.Password;

    public EditProfileDialog(AccountModel account)
    {
        InitializeComponent();
        tbFirstName.Text         = account.FirstName;
        tbLastName.Text          = account.LastName;
        tbEmail.Text             = account.Email;
        tbPhone.Text             = account.Phone;
        tbAddress.Text           = account.Address;
        dpBirthdate.SelectedDate = account.Birthdate;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        errFirstName.Visibility = Visibility.Collapsed;
        errLastName.Visibility  = Visibility.Collapsed;
        errEmail.Visibility     = Visibility.Collapsed;
        errPhone.Visibility     = Visibility.Collapsed;
        errAddress.Visibility   = Visibility.Collapsed;
        errPassword.Visibility  = Visibility.Collapsed;

        bool valid = true;
        if (!IsValidName(FirstName))    { errFirstName.Visibility = Visibility.Visible; valid = false; }
        if (!IsValidName(LastName))     { errLastName.Visibility  = Visibility.Visible; valid = false; }
        if (!IsValidEmail(Email))       { errEmail.Visibility     = Visibility.Visible; valid = false; }
        if (!IsValidPhone(Phone))       { errPhone.Visibility     = Visibility.Visible; valid = false; }
        if (Address.Length < 5)         { errAddress.Visibility   = Visibility.Visible; valid = false; }
        if (!IsValidPassword(Password)) { errPassword.Visibility  = Visibility.Visible; valid = false; }

        if (valid) DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;

    private static bool IsValidName(string name) =>
        name.Length >= 3 && name.Length <= 30 &&
        Regex.IsMatch(name, @"^[a-zA-ZäöüÄÖÜß\- ]+$");

    private static bool IsValidEmail(string email) =>
        Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]{2,}$");

    private static bool IsValidPhone(string phone)
    {
        var stripped = phone.Replace(" ", "").Replace("-", "");
        return stripped.Length >= 7 && stripped.Length <= 15 &&
               Regex.IsMatch(stripped, @"^\+?[0-9]+$");
    }

    private static bool IsValidPassword(string pw) =>
        pw.Length >= 8 && pw.Any(char.IsUpper) && pw.Any(char.IsDigit);
}
