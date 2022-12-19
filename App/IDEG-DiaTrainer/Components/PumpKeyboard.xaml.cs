using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Components
{
    public class KeyboardViewModel : INotifyPropertyChanged
    {
        private string _Units = "";
        public string Units { get { return _Units; } set { _Units = value; OnPropertyChanged("Units"); } }

        private string _TextValue = "";
        public string TextValue {
            get
            {
                return _TextValue;
            }
            set
            {
                _TextValue = value;
                if (!Double.TryParse(_TextValue, CultureInfo.InvariantCulture, out _NumericValue))
                    _NumericValue = 0;

                OnPropertyChanged("TextValue");
                OnPropertyChanged("NumericValue");
            }
        }

        private double _NumericValue = 0.0;
        public double NumericValue {
            get
            {
                return _NumericValue;
            }
            set
            {
                _NumericValue = value;
                _TextValue = _NumericValue.ToString();

                OnPropertyChanged("TextValue");
                OnPropertyChanged("NumericValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class PumpKeyboard : Grid
    {
        public string Units { get { return viewModel.Units; } set { viewModel.Units = value; } }

        public KeyboardViewModel viewModel { get; private set; } = new KeyboardViewModel();

        public PumpKeyboard()
        {
            BindingContext = viewModel;

            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            string txt = ((Button)sender).Text;
            if (txt.Length == 0)
                return;

            if (txt[0] >= '0' && txt[0] <= '9')
            {
                // allow just one decimal place
                if (!viewModel.TextValue.Contains('.') || viewModel.TextValue.IndexOf('.') == viewModel.TextValue.Length - 1)
                    viewModel.TextValue += txt;
            }
            else if (txt[0] == '.' && !viewModel.TextValue.Contains('.'))
                viewModel.TextValue += txt;
            else if (txt[0] == 'X' && viewModel.TextValue.Length > 0)
                viewModel.TextValue = viewModel.TextValue.Substring(0, viewModel.TextValue.Length - 1);
        }

        public void Reset()
        {
            viewModel.TextValue = "";
        }

        public double GetEnteredValue()
        {
            return viewModel.NumericValue;
        }
    }
}