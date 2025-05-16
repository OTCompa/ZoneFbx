using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Configuration;
using System.ComponentModel;
namespace ZoneFbx.GUI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        private Configuration Config;
        private Config ExportConfig;

        public string _specularFactor;
        public string SpecularFactor
        {
            get => _specularFactor;
            set
            {
                _specularFactor = value;
                OnPropertyChanged(nameof(SpecularFactor));
            }
        }

        public string _normalFactor;
        public string NormalFactor
        {
            get => _normalFactor;
            set
            {
                _normalFactor = value;
                OnPropertyChanged(nameof(NormalFactor));
            }
        }
        public string _lightIntensityFactor;
        public string LightIntensityFactor
        {
            get => _lightIntensityFactor;
            set
            {
                _lightIntensityFactor = value;
                OnPropertyChanged(nameof(LightIntensityFactor));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public ConfigWindow(Configuration config, Config exportConfig)
        {
            Config = config;
            ExportConfig = exportConfig;
            SpecularFactor = ExportConfig.SpecularFactor;
            NormalFactor = ExportConfig.NormalFactor;
            LightIntensityFactor = ExportConfig.LightIntensityFactor;


            InitializeComponent();
            DataContext = this;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveConfig(object sender, EventArgs e)
        {
            var valid = double.TryParse(SpecularFactor, out var specularFactor);
            valid = valid && specularFactor >= 0 && specularFactor <= 1;
            if (!valid) {
                MessageBox.Show("Invalid specular factor.");
                return;
            }

            valid = double.TryParse(NormalFactor, out var normalFactor);
            valid = valid && normalFactor >= 0 && normalFactor <= 1;
            if (!valid) { 
                MessageBox.Show("Invalid normal factor.");
                return;
            }

            valid = double.TryParse(LightIntensityFactor, out var lightIntensityFactor);
            valid = valid && lightIntensityFactor >= 0;
            if (!valid)
            {
                MessageBox.Show("Invalid light intensity factor.");
                return;
            }

            ExportConfig.SpecularFactor = SpecularFactor;
            ExportConfig.NormalFactor = NormalFactor;
            ExportConfig.LightIntensityFactor = LightIntensityFactor;
            Config.Save();

            Close();
        }
    }
}
