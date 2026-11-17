using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace transportation_problems_app.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private bool _IsDummyRes = false;
        private bool _IsDummyNeed = false;
        private double _DummyResValue = 0;
        private double _DummyNeedValue = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void NumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string futureText = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                                            .Insert(textBox.SelectionStart, e.Text);

            e.Handled = !IsValidDecimal(futureText);
        }

        private bool IsValidDecimal(string text)
        {
            if (string.IsNullOrEmpty(text)) return true;

            if (text.Contains(" ")) return false;

            string normalizedText = text.Replace(',', '.');

            return double.TryParse(normalizedText, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out _);
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            A1B1.Clear();
            A1B2.Clear();
            A1B3.Clear();
            A1B4.Clear();
            A1B5.Clear();
            A2B1.Clear();
            A2B2.Clear();
            A2B3.Clear();
            A2B4.Clear();
            A2B5.Clear();
            A3B1.Clear();
            A3B2.Clear();
            A3B3.Clear();
            A3B4.Clear();
            A3B5.Clear();

            NeedsB1.Clear();
            NeedsB2.Clear();
            NeedsB3.Clear();
            NeedsB4.Clear();
            NeedsB5.Clear();

            ReservesA1.Clear();
            ReservesA2.Clear();
            ReservesA3.Clear();
        }

        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(A1B1.Text) || string.IsNullOrWhiteSpace(A1B2.Text) || string.IsNullOrWhiteSpace(A1B3.Text) || string.IsNullOrWhiteSpace(A1B4.Text) || string.IsNullOrWhiteSpace(A1B5.Text) ||
                string.IsNullOrWhiteSpace(A2B1.Text) || string.IsNullOrWhiteSpace(A2B2.Text) || string.IsNullOrWhiteSpace(A2B3.Text) || string.IsNullOrWhiteSpace(A2B4.Text) || string.IsNullOrWhiteSpace(A2B5.Text) ||
                string.IsNullOrWhiteSpace(A3B1.Text) || string.IsNullOrWhiteSpace(A3B2.Text) || string.IsNullOrWhiteSpace(A3B3.Text) || string.IsNullOrWhiteSpace(A3B4.Text) || string.IsNullOrWhiteSpace(A3B5.Text) ||
                string.IsNullOrWhiteSpace(NeedsB1.Text) || string.IsNullOrWhiteSpace(NeedsB2.Text) || string.IsNullOrWhiteSpace(NeedsB3.Text) || string.IsNullOrWhiteSpace(NeedsB4.Text) || string.IsNullOrWhiteSpace(NeedsB5.Text) ||
                string.IsNullOrWhiteSpace(ReservesA1.Text) || string.IsNullOrWhiteSpace(ReservesA2.Text) || string.IsNullOrWhiteSpace(ReservesA3.Text))
            {
                MessageBox.Show(Application.Current.MainWindow,
                    "Заполните все поля!",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }

            double ResA1 = Convert.ToDouble(ReservesA1.Text);
            double ResA2 = Convert.ToDouble(ReservesA2.Text);
            double ResA3 = Convert.ToDouble(ReservesA3.Text);

            double NeedB1 = Convert.ToDouble(NeedsB1.Text);
            double NeedB2 = Convert.ToDouble(NeedsB2.Text);
            double NeedB3 = Convert.ToDouble(NeedsB3.Text);
            double NeedB4 = Convert.ToDouble(NeedsB4.Text);
            double NeedB5 = Convert.ToDouble(NeedsB5.Text);

            double ResSum = ResA1 + ResA2 + ResA3;
            double NeedSum = NeedB1 + NeedB2 + NeedB3 + NeedB4 + NeedB5;

            if (ResSum == NeedSum)
            {
                MessageBox.Show(Application.Current.MainWindow,
                    "Матрица сбалансирована!",
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
            else 
            {
                string message;
                if (ResSum < NeedSum)
                {
                    _DummyResValue = NeedSum - ResSum;
                    _IsDummyRes = true;
                    message = $"Матрица была не сбалансирована, добавлен фиктивный поставщик со значением: {_DummyResValue}";
                }
                else 
                {
                    _DummyNeedValue = ResSum - NeedSum;
                    _IsDummyNeed = true;
                    message = $"Матрица была не сбалансирована, добавлен фиктивный потребитель со значением: {_DummyNeedValue}";
                }

                MessageBox.Show(Application.Current.MainWindow,
                    message,
                    "Успех",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }
    }
}
