using BankApp.Models;
using System.Windows;

namespace BankApp.Views;

public partial class EditClientDialog : Window
{
    public string FirstName => tbFirstName.Text.Trim();
    public string LastName => tbLastName.Text.Trim();
    public string Email => tbEmail.Text.Trim();
    public string Phone => tbPhone.Text.Trim();
    public string Address => tbAddress.Text.Trim();

    public EditClientDialog(AccountModel client)
    {
        InitializeComponent();

        tbFirstName.Text = client.FirstName;
        tbLastName.Text = client.LastName;
        tbEmail.Text = client.Email;
        tbPhone.Text = client.Phone;
        tbAddress.Text = client.Address;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (FirstName.Length < 3 || LastName.Length < 3 ||
            string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Phone) || string.IsNullOrWhiteSpace(Address))
        {
            MessageBox.Show("Bitte alle Felder korrekt ausfüllen.\nName min. 3 Zeichen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}