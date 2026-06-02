using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BankApp.Views
{
    public partial class AmountPicker : UserControl
    {
        #region MitKiGeneriert
        public AmountPicker()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AmountInCentsProperty =
            DependencyProperty.Register(
                nameof(AmountInCents),
                typeof(int),
                typeof(AmountPicker),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnAmountChanged));

        public int AmountInCents
        {
            get => (int)GetValue(AmountInCentsProperty);
            set => SetValue(AmountInCentsProperty, value);
        }

        public int Euros
        {
            get => AmountInCents / 100;
            set => AmountInCents = value * 100 + Cents;
        }

        public int Cents
        {
            get => AmountInCents % 100;
            set => AmountInCents = Euros * 100 + value;
        }

        private static void OnAmountChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = (AmountPicker)d;

            control.EuroTextBox.Text = control.Euros.ToString();
            control.CentTextBox.Text = control.Cents.ToString("00");
        }

        private void EuroUp_Click(object sender, RoutedEventArgs e)
        {
            AmountInCents += 100;
        }

        private void EuroDown_Click(object sender, RoutedEventArgs e)
        {
            if (AmountInCents >= 100)
                AmountInCents -= 100;
        }

        private void CentUp_Click(object sender, RoutedEventArgs e)
        {
            AmountInCents += 1;
        }

        private void CentDown_Click(object sender, RoutedEventArgs e)
        {
            if (AmountInCents > 0)
                AmountInCents -= 1;
        }
        #endregion
    }
}
