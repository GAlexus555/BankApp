using System;
using System.Windows;

namespace BankApp.Views;

public partial class AddClientDialog : Window
{
    public string FirstName => tbFirstName.Text.Trim();
    public string LastName => tbLastName.Text.Trim();
    public string Email => tbEmail.Text.Trim();
    public string Password => pbPassword.Password;
    public string Phone => tbPhone.Text.Trim();
    public string Address => tbAddress.Text.Trim();
    public DateTime Birthdate => dpBirthdate.SelectedDate ?? DateTime.Today.AddYears(-18);

    public AddClientDialog()
    {
        InitializeComponent();
        dpBirthdate.SelectedDate = DateTime.Today.AddYears(-18);
    }

    private void Create_Click(object sender, RoutedEventArgs e)
    {
        if (FirstName.Length < 3 || LastName.Length < 3 ||
            string.IsNullOrWhiteSpace(Email) || Password.Length < 8 ||
            string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Address))
        {
            MessageBox.Show("Bitte alle Felder korrekt ausfüllen.\nName min. 3 Zeichen, Passwort min. 8 Zeichen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}
