using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace transportation_problems_app.Classes
{
    public class TransportRow : INotifyPropertyChanged
    {
        // Имя поставщика (A1, A2...)
        public string Name { get; set; }

        // Запас (выносим в конец)
        private double _supply;
        public double Supply
        {
            get => _supply;
            set { _supply = value; OnPropertyChanged(); }
        }

        // Массив с тарифами (ячейки с цифрами). 
        // ObservableCollection нужна, чтобы DataGrid видел изменения
        public ObservableCollection<double> Costs { get; set; } = new ObservableCollection<double>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
