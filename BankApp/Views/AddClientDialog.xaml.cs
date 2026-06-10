using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace BankApp.Views;

public partial class AddClientDialog : Window
{
    public string FirstName => tbFirstName.Text.Trim();
    public string LastName  => tbLastName.Text.Trim();
    public string Email     => tbEmail.Text.Trim();
    public string Password  => pbPassword.Password;
    public string Phone     => tbPhone.Text.Trim();
    public string Address   => tbAddress.Text.Trim();
    public DateTime Birthdate => dpBirthdate.SelectedDate ?? DateTime.Today.AddYears(-18);

    public AddClientDialog()
    {
        InitializeComponent();
        dpBirthdate.SelectedDate = DateTime.Today.AddYears(-18);
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        errFirstName.Visibility = Visibility.Collapsed;
        errLastName.Visibility  = Visibility.Collapsed;
        errEmail.Visibility     = Visibility.Collapsed;
        errPassword.Visibility  = Visibility.Collapsed;
        errPhone.Visibility     = Visibility.Collapsed;
        errAddress.Visibility   = Visibility.Collapsed;

        bool valid = true;
        if (FirstName.Length < 2)    { errFirstName.Visibility = Visibility.Visible; valid = false; }
        if (LastName.Length < 2)     { errLastName.Visibility  = Visibility.Visible; valid = false; }
        if (!IsValidEmail(Email))    { errEmail.Visibility     = Visibility.Visible; valid = false; }
        if (Password.Length < 8)     { errPassword.Visibility  = Visibility.Visible; valid = false; }
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
