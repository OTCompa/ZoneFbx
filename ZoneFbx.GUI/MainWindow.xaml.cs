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

        // bindings for window
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

        private bool _execInProgress = false;
        public bool ExecInProgress
        {
            get => _execInProgress;
            set
            {
                _execInProgress = value;
                OnPropertyChanged(nameof(ExecInProgress));
            }
        }

        // levels
        public class ComboBoxItem
        {
            public string Value { get; set; }
            public string DisplayValue { get; set; }
            public ComboBoxItem(string value, string displayValue)
            {
                Value = value;
                DisplayValue = displayValue;
            }
        }

        // dropdown elements
        public ObservableCollection<ComboBoxItem> Levels { get; set; } = new();
        public ListCollectionView FilteredLevels { get; private set; }

        private string _level = "";
        public string Level
        {
            get => _level;
            set
            {
                var match = Levels.FirstOrDefault(i => i.DisplayValue == value);
                if (match != null)
                    _level = match.Value;
                else
                    _level = value;

                OnPropertyChanged(nameof(Level));
                FilteredLevels.Refresh();
            }
        }

        // flags
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

        private bool _enableLighting = false;
        public bool EnableLighting
        {
            get => _enableLighting;
            set
            {
                _enableLighting = value;
                OnPropertyChanged(nameof(EnableLighting));
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

        private bool _enableJsonExport = false;
        public bool EnableJsonExport
        {
            get => _enableJsonExport;
            set
            {
                _enableJsonExport = value;
                OnPropertyChanged(nameof(EnableJsonExport));
            }
        }

        private bool _enableBlend = false;
        public bool EnableBlend
        {
            get => _enableBlend;
            set
            {
                _enableBlend = value;
                OnPropertyChanged(nameof(EnableBlend));
            }
        }

        private bool _enableMTMap = false;
        public bool EnableMTMap
        {
            get => _enableMTMap;
            set
            {
                _enableMTMap = value;
                OnPropertyChanged(nameof(EnableMTMap));
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            InitializeComponent();
            FilteredLevels = new ListCollectionView(Levels);
            FilteredLevels.Filter = LevelFilter;
            DataContext = this;
            var defaultDir = "C:\\Program Files (x86)\\SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack";
            if (Directory.Exists(defaultDir))
            {
                GamePath = defaultDir;
            }
        }

        private async void TryResolvingLumina()
        {
            await Task.Run(() =>
            {
                try
                {
                    data = new Lumina.GameData(GamePath);
                } catch (Exception ex)
                {
                    ConsoleString = $"Unable to resolve game data: {ex.Message}\nYou might want to check if you correctly set the path to the sqpack folder.";
                    return;
                }

                ConsoleString = "";
                Dispatcher.BeginInvoke(Levels.Clear);
                var territoryType = data.GetExcelSheet<TerritoryType>();
                if (territoryType == null) {
                    ConsoleString = $"Unable to get excel datasheet.";
                    return;
                }
                foreach (var row in territoryType.Where(territory => !String.IsNullOrEmpty(territory.PlaceName.ValueNullable?.Name.ExtractText())))
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        Levels.Add(new ComboBoxItem(row.Bg.ToString(), $"{row.PlaceNameZone.Value.Name} {row.PlaceName.Value.Name} ({row.Bg})"));
                    });
                }
                Dispatcher.BeginInvoke(() =>
                {
                    Level = "";
                });
            });

        }

        private bool LevelFilter(object item)
        {
            if (item is not ComboBoxItem comboItem) return false;
            if (string.IsNullOrWhiteSpace(Level)) return true;

            return comboItem.DisplayValue.IndexOf(Level, StringComparison.OrdinalIgnoreCase) >= 0;
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
            if (textBox == null) return;
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
            if (textBox == null) return;
            OutputPath = textBox.Text;
        }

        private void AppendConsole(string line)
        {
            ConsoleString += $"{line}\n";
            Dispatcher.BeginInvoke(ConsoleTextBox.ScrollToEnd);
        }

        private async void ExportMap(object sender, RoutedEventArgs e)
        {
            ExecInProgress = true;
            var argLevel = Level;
            var argOutput = OutputPath.EndsWith(System.IO.Path.DirectorySeparatorChar) ? OutputPath : OutputPath + System.IO.Path.DirectorySeparatorChar;
            var argFlags = string.Concat([
                "-",
                ..new[]{
                    EnableLightshaft ? "l" : "",
                    EnableFestival ? "f" : "",
                    DisableBaking ? "b" : "",
                    EnableJsonExport ? "j" : "",
                    EnableLighting ? "i" : "",
                    EnableBlend ? "s" : "",
                    EnableMTMap ? "m" : "",
                    }
                ]);

            if (argFlags == "-") argFlags = "";

            ConsoleString = "";
            ConsoleString += $"ZoneFbx \"{GamePath}\" {argLevel} \"{argOutput}\\\" {argFlags}\n";
            try
            {
                var result = await CliWrap.Cli.Wrap("ZoneFbx")
                    .WithArguments([GamePath, argLevel, argOutput, argFlags])
                    .WithStandardOutputPipe(PipeTarget.ToDelegate(AppendConsole))
                    .WithStandardErrorPipe(PipeTarget.ToDelegate(AppendConsole))
                    .ExecuteAsync();
                ConsoleString += $"ZoneFbx was {(!result.IsSuccess ? "un" : "")}able to successfully exit.";
            } catch (Exception ex)
            {
                ConsoleString += $"ZoneFbx failed to extract {Level} with the error {ex.Message}\n";
                ConsoleString += $"Trace:\n{ex.StackTrace}\n";
            }
            ExecInProgress = false;
        }
    }
}