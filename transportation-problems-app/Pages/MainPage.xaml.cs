using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace transportation_problems_app.Pages
{
    public partial class MainPage : Page
    {
        // Переменные для фиктивных узлов 
        private bool _IsDummyRes = false;
        private bool _IsDummyNeed = false;
        private double _DummyResValue = 0;
        private double _DummyNeedValue = 0;

        // Переменные для динамической матрицы 
        private int _rows = 3;
        private int _cols = 5; 

        private TextBox[,] _cells;      // Тарифы
        private TextBox[] _supplies;    // Запасы (Reserves)
        private TextBox[] _demands;     // Потребности (Needs)


        public MainPage()
        {
            InitializeComponent();

            BuildMatrix();
        }


        private void BuildMatrix()
        {
            MainGrid.Children.Clear();
            MainGrid.RowDefinitions.Clear();
            MainGrid.ColumnDefinitions.Clear();

            _cells = new TextBox[_rows, _cols];
            _supplies = new TextBox[_rows];
            _demands = new TextBox[_cols];

            // Колонки
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto }); // Заголовки A
            for (int j = 0; j < _cols; j++)
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(75) });
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Запасы

            // Строки
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Заголовки B
            for (int i = 0; i < _rows; i++)
                MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            MainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Потребности

            // Заголовки "Пункты поставки", "Пункты потребления"
            AddTextBlock("Пункты поставки", 0, 0, true);
            for (int j = 0; j < _cols; j++) AddTextBlock($"B{j + 1}", 0, j + 1);
            AddTextBlock("Запасы", 0, _cols + 1, true);

            // Строки A и запасы
            for (int i = 0; i < _rows; i++)
            {
                AddTextBlock($"A{i + 1}", i + 1, 0);
                _supplies[i] = CreateTextBox(i + 1, _cols + 1);
            }

            // Строка "Потребности" и значения B
            AddTextBlock("Потребности", _rows + 1, 0, true);
            for (int j = 0; j < _cols; j++)
                _demands[j] = CreateTextBox(_rows + 1, j + 1);

            // Тарифы
            for (int i = 0; i < _rows; i++)
                for (int j = 0; j < _cols; j++)
                    _cells[i, j] = CreateTextBox(i + 1, j + 1);
        }


        private void AddTextBlock(string text, int row, int col, bool isBold = false)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = 18,
                FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(tb, row);
            Grid.SetColumn(tb, col);
            MainGrid.Children.Add(tb);
        }
        private TextBox CreateTextBox(int row, int col)
        {
            var txt = new TextBox { VerticalContentAlignment = VerticalAlignment.Center, FontSize = 14, Margin = new Thickness(3) };
            txt.PreviewTextInput += NumberTextBox_PreviewTextInput;
            Grid.SetRow(txt, row);
            Grid.SetColumn(txt, col);
            MainGrid.Children.Add(txt);
            return txt;
        }


        private void AddRowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_rows >= 10) return;
            SaveCurrentData(out var c, out var s, out var d);
            _rows++; BuildMatrix(); LoadData(c, s, d);
        }
        private void RemoveRowBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_rows <= 1) return;
            SaveCurrentData(out var c, out var s, out var d);
            _rows--; BuildMatrix(); LoadData(c, s, d);
        }
        private void AddColBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_cols >= 10) return;
            SaveCurrentData(out var c, out var s, out var d);
            _cols++; BuildMatrix(); LoadData(c, s, d);
        }
        private void RemoveColBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_cols <= 1) return;
            SaveCurrentData(out var c, out var s, out var d);
            _cols--; BuildMatrix(); LoadData(c, s, d);
        }


        private void SaveCurrentData(out string[,] cells, out string[] sup, out string[] dem)
        {
            cells = new string[_rows, _cols];
            sup = new string[_rows];
            dem = new string[_cols];

            for (int i = 0; i < _rows; i++)
            {
                sup[i] = _supplies[i].Text;
                for (int j = 0; j < _cols; j++) cells[i, j] = _cells[i, j].Text;
            }
            for (int j = 0; j < _cols; j++) dem[j] = _demands[j].Text;
        }
        private void LoadData(string[,] oldCells, string[] oldSup, string[] oldDem)
        {
            for (int i = 0; i < _rows; i++)
            {
                if (i < oldSup.Length) _supplies[i].Text = oldSup[i];
                for (int j = 0; j < _cols; j++)
                    if (i < oldCells.GetLength(0) && j < oldCells.GetLength(1)) _cells[i, j].Text = oldCells[i, j];
            }
            for (int j = 0; j < _cols; j++)
                if (j < oldDem.Length) _demands[j].Text = oldDem[j];
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


        private double[,] SolveNorthWest(double[] originalSupplies, double[] originalDemands, int rows, int cols)
        {
            double[,] allocation = new double[rows, cols];

            double[] supplies = (double[])originalSupplies.Clone();
            double[] demands = (double[])originalDemands.Clone();

            int i = 0; // Индекс строки (поставщик)
            int j = 0; // Индекс столбца (потребитель)

            while (i < rows && j < cols)
            {
                double amount = Math.Min(supplies[i], demands[j]);
                allocation[i, j] = amount;

                supplies[i] -= amount;
                demands[j] -= amount;

                // Так как мы работаем с дробными числами, используем погрешность для сравнения с нулем
                bool supplyEmpty = Math.Abs(supplies[i]) < 0.0001;
                bool demandEmpty = Math.Abs(demands[j]) < 0.0001;

                if (supplyEmpty && demandEmpty)
                {
                    i++;
                    j++;
                }
                else if (supplyEmpty)
                {
                    i++;
                }
                else if (demandEmpty)
                {
                    j++;
                }
            }

            return allocation;
        }


        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _rows; i++)
            {
                _supplies[i].Clear();
                for (int j = 0; j < _cols; j++)
                    _cells[i, j].Clear();
            }
            for (int j = 0; j < _cols; j++)
                _demands[j].Clear();
        }
        private void SolveBtn_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка на пустые поля
            for (int i = 0; i < _rows; i++)
            {
                if (string.IsNullOrWhiteSpace(_supplies[i].Text))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Заполните все поля (Запасы)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                for (int j = 0; j < _cols; j++)
                {
                    if (string.IsNullOrWhiteSpace(_cells[i, j].Text))
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Заполните все поля (Тарифы)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            for (int j = 0; j < _cols; j++)
            {
                if (string.IsNullOrWhiteSpace(_demands[j].Text))
                {
                    MessageBox.Show(Application.Current.MainWindow, "Заполните все поля (Потребности)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // 2. Сбор данных
            double[] supplies = new double[_rows];
            double[] demands = new double[_cols];
            double[,] costs = new double[_rows, _cols];

            double ResSum = 0;
            double NeedSum = 0;

            for (int i = 0; i < _rows; i++)
            {
                supplies[i] = Convert.ToDouble(_supplies[i].Text);
                ResSum += supplies[i];
                for (int j = 0; j < _cols; j++)
                {
                    costs[i, j] = Convert.ToDouble(_cells[i, j].Text);
                }
            }

            for (int j = 0; j < _cols; j++)
            {
                demands[j] = Convert.ToDouble(_demands[j].Text);
                NeedSum += demands[j];
            }

            // 3. Проверка баланса 
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

            double[,] allocationMatrix = new double[_rows, _cols];

            if (MethodComboBox.SelectedIndex == 0) 
            {
                double[,] resultMatrix = SolveNorthWest(supplies, demands, _rows, _cols);
                // Вывод в MessageBox
            }
        }
    }
}