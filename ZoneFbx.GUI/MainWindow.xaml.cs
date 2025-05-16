using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Lumina.Excel.Sheets;
using CliWrap;
using System.IO;
using System.Configuration;
using Action = System.Action;
using System.Collections.Specialized;

namespace ZoneFbx.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly Configuration Config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private readonly Config ExportConfig;
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

        public string GamePath
        {
            get => ExportConfig.GamePath;
            set
            {
                ExportConfig.GamePath = value;
                OnPropertyChanged(nameof(GamePath));
                Config.Save();
            }
        }

        public string OutputPath
        {
            get => ExportConfig.OutputPath;
            set
            {
                ExportConfig.OutputPath = value;
                OnPropertyChanged(nameof(OutputPath));
                Config.Save();
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
        public bool EnableLightshaft
        {
            get => ExportConfig.EnableLightshaft;
            set
            {
                ExportConfig.EnableLightshaft = value;
                OnPropertyChanged(nameof(EnableLightshaft));
                Config.Save();
            }
        }

        public bool EnableLighting
        {
            get => ExportConfig.EnableLighting;
            set
            {
                ExportConfig.EnableLighting = value;
                OnPropertyChanged(nameof(EnableLighting));
                Config.Save();
            }
        }

        public bool EnableFestival
        {
            get => ExportConfig.EnableFestival;
            set
            {
                ExportConfig.EnableFestival = value;
                OnPropertyChanged(nameof(EnableFestival));
                Config.Save();
            }
        }

        public bool EnableJsonExport
        {
            get => ExportConfig.EnableJsonExport;
            set
            {
                ExportConfig.EnableJsonExport = value;
                OnPropertyChanged(nameof(EnableJsonExport));
                Config.Save();
            }
        }

        public bool EnableBlend
        {
            get => ExportConfig.EnableBlend;
            set
            {
                ExportConfig.EnableBlend = value;
                OnPropertyChanged(nameof(EnableBlend));
                Config.Save();
            }
        }

        public bool EnableMTMap
        {
            get => ExportConfig.EnableMTMap;
            set
            {
                ExportConfig.EnableMTMap = value;
                OnPropertyChanged(nameof(EnableMTMap));
                Config.Save();
            }
        }

        public bool DisableBaking
        {
            get => ExportConfig.DisableBaking;
            set
            {
                ExportConfig.DisableBaking = value;
                OnPropertyChanged(nameof(DisableBaking));
                Config.Save();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            if (Config.GetSection("ExportConfig") is null)
            {
                Config.Sections.Add("ExportConfig", new Config());
                Config.Save();
            }

            ExportConfig = (Config)Config.GetSection("ExportConfig");

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