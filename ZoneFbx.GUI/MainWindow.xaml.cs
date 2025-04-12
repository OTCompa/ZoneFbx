using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
using Lumina.Excel.Sheets;
using CliWrap;
using System.IO;
using Action = System.Action;

namespace ZoneFbx.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private Lumina.GameData? data;
        private string _consoleString = "";
        public string ConsoleString
        {
            get => _consoleString;
            set
            {
                _consoleString = value;
                OnPropertyChanged(nameof(ConsoleString));
            }
        }

        private string _gamePath = "";
        public string GamePath
        {
            get => _gamePath;
            set
            {
                _gamePath = value;
                OnPropertyChanged(nameof(GamePath));
            }
        }

        private string _outputPath = "";
        public string OutputPath
        {
            get => _outputPath;
            set
            {
                _outputPath = value;
                OnPropertyChanged(nameof(OutputPath));
            }
        }

        public ObservableCollection<string> Levels { get; set; } = new();
        private string _level = "";
        public string Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged(nameof(Level));
            }
        }

        private bool _enableLightshaft = false;
        public bool EnableLightshaft
        {
            get => _enableLightshaft;
            set
            {
                _enableLightshaft = value;
                OnPropertyChanged(nameof(EnableLightshaft));
            }
        }

        private bool _enableFestival = false;
        public bool EnableFestival
        {
            get => _enableFestival;
            set
            {
                _enableFestival = value;
                OnPropertyChanged(nameof(EnableFestival));
            }
        }
        private bool _disableBaking = false;
        public bool DisableBaking
        {
            get => _disableBaking;
            set
            {
                _disableBaking = value;
                OnPropertyChanged(nameof(DisableBaking));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void TryResolvingLumina()
        {
            try
            {
                data = new Lumina.GameData(GamePath);
            } catch (Exception ex)
            {
                ConsoleString = "Invalid game path";
                return;
            }

            ConsoleString = "";
            Levels.Clear();
            var territoryType = data.GetExcelSheet<TerritoryType>();
            foreach (var row in territoryType.Where(territory => !String.IsNullOrEmpty(territory.PlaceName.ValueNullable?.Name.ExtractText())))
            {
                Levels.Add($"{row.Bg} {row.PlaceNameZone.Value.Name} {row.PlaceName.Value.Name}");
            }

        }

        private void SelectGamePath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog{};

            if (folderDialog.ShowDialog() == true)
            {
                GamePath = folderDialog.FolderName;
                TryResolvingLumina();
            }
        }

        private void GamePathChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            GamePath = textBox.Text;
            TryResolvingLumina();
        }
        private void SelectOutputPath(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFolderDialog { };

            if (folderDialog.ShowDialog() == true)
            {
                OutputPath = folderDialog.FolderName;
            }
        }

        private void OutputPathChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            OutputPath = textBox.Text;
        }

        private void AppendConsole(string line)
        {
            ConsoleString += $"{line}\n";
            Dispatcher.BeginInvoke(ConsoleTextBox.ScrollToEnd);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var argLevel = Level.Split(' ')[0];
            Debug.WriteLine(argLevel);
            var argOutput = OutputPath.EndsWith(System.IO.Path.DirectorySeparatorChar) ? OutputPath : OutputPath + System.IO.Path.DirectorySeparatorChar;
            var argFlags = "";
            if (EnableLightshaft) argFlags += "l";
            if (EnableFestival) argFlags += "f";
            if (DisableBaking) argFlags += "b";
            if (argFlags.Length > 0) argFlags = "-" + argFlags;

            ConsoleString = "";
            ConsoleString += $"ZoneFbx {GamePath} {argLevel} {argOutput} {argFlags}";
            var result = await CliWrap.Cli.Wrap("bin\\ZoneFbx")
                .WithArguments([GamePath, argLevel, argOutput, argFlags])
                .WithStandardOutputPipe(PipeTarget.ToDelegate(AppendConsole))
                .ExecuteAsync();
        }
    }
}