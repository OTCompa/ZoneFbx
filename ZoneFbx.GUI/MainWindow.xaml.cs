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
using System.Runtime.CompilerServices;

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
            set => SetConfig(() => ExportConfig.GamePath = value);
        }

        public string OutputPath
        {
            get => ExportConfig.OutputPath;
            set => SetConfig(() => ExportConfig.OutputPath = value);
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
                _level = match?.Value ?? value;
                LevelComboBox.SelectedItem = null;

                OnPropertyChanged(nameof(Level));
                FilteredLevels.Refresh();
            }
        }

        // flags
        public bool EnableMainMap
        {
            get => ExportConfig.EnableMainMap;
            set => SetConfig(() => ExportConfig.EnableMainMap = value);
        }

        public bool EnableLightshaft
        {
            get => ExportConfig.EnableLightshaft;
            set => SetConfig(() => ExportConfig.EnableLightshaft = value);
        }

        public bool EnableLighting
        {
            get => ExportConfig.EnableLighting;
            set => SetConfig(() => ExportConfig.EnableLighting = value);
        }

        public bool EnableFestival
        {
            get => ExportConfig.EnableFestival;
            set => SetConfig(() => ExportConfig.EnableFestival = value);
        }

        public bool EnableJsonExport
        {
            get => ExportConfig.EnableJsonExport;
            set => SetConfig(() => ExportConfig.EnableJsonExport = value);
        }

        public bool EnableBlend
        {
            get => ExportConfig.EnableBlend;
            set => SetConfig(() => ExportConfig.EnableBlend = value);
        }

        public bool EnableMTMap
        {
            get => ExportConfig.EnableMTMap;
            set => SetConfig(() => ExportConfig.EnableMTMap = value);
        }

        public bool EnableCollision
        {
            get => ExportConfig.EnableCollision;
            set => SetConfig(() => ExportConfig.EnableCollision = value);
        }

        public bool EnableCollisionVariants
        {
            get => ExportConfig.EnableCollisionVariants;
            set => SetConfig(() => ExportConfig.EnableCollisionVariants = value);
        }

        public bool DisableBaking
        {
            get => ExportConfig.DisableBaking;
            set => SetConfig(() => ExportConfig.DisableBaking = value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Apply a setter against ExportConfig, raise change notification, persist. CallerMemberName
        // lets each property delegate here with a single lambda instead of repeating the boilerplate.
        private void SetConfig(System.Action setter, [CallerMemberName] string propertyName = "")
        {
            setter();
            OnPropertyChanged(propertyName);
            Config.Save();
        }

        private ConfigWindow configWindow = null!;

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

        private void OpenConfigWindow(object sender, RoutedEventArgs e)
        {
            configWindow = new(Config, ExportConfig);
            configWindow.ShowDialog();
        }

        private async void TryResolvingLumina()
        {
            List<ComboBoxItem>? items = null;
            string? errorMessage = null;

            await Task.Run(() =>
            {
                try
                {
                    data = new Lumina.GameData(GamePath, new Lumina.LuminaOptions() { PanicOnSheetChecksumMismatch = false });
                }
                catch (Exception ex)
                {
                    errorMessage = $"Unable to resolve game data: {ex.Message}\nYou might want to check if you correctly set the path to the sqpack folder.";
                    return;
                }

                var territoryType = data.GetExcelSheet<TerritoryType>();
                if (territoryType == null)
                {
                    errorMessage = "Unable to get excel datasheet.";
                    return;
                }

                // Build the entire list off-thread, then push to the UI in one go to avoid flooding
                // the dispatcher with one item-add per row (which made the GUI freeze on large sheets).
                items = territoryType
                    .Where(t => !string.IsNullOrEmpty(t.PlaceName.ValueNullable?.Name.ExtractText()))
                    .Select(t => new ComboBoxItem(
                        t.Bg.ToString(),
                        $"{t.PlaceNameZone.ValueNullable?.Name.ExtractText() ?? ""} {t.PlaceName.Value.Name.ExtractText()} ({t.Bg})"))
                    .DistinctBy(item => item.DisplayValue)
                    .ToList();
            });

            // We're back on the UI thread here.
            if (errorMessage != null)
            {
                ConsoleString = errorMessage;
                return;
            }

            ConsoleString = "";
            Levels.Clear();
            if (items != null)
            {
                foreach (var item in items) Levels.Add(item);
            }
            Level = "";
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
                    EnableCollision ? "c" : "",
                    !EnableMainMap ? "n" : "",
                    !EnableCollisionVariants ? "v" : "",
                    }
                ]);

            if (argFlags == "-") argFlags = "";

            // there's probably a better way to do this...
            var argVars = new List<string>();

            if (!string.IsNullOrEmpty(ExportConfig.SpecularFactor))
            {
                argVars.Add("--specular");
                argVars.Add(ExportConfig.SpecularFactor);
            }

            if (!string.IsNullOrEmpty(ExportConfig.NormalFactor))
            {
                argVars.Add("--normal");
                argVars.Add(ExportConfig.NormalFactor);
            }

            if (!string.IsNullOrEmpty(ExportConfig.LightIntensityFactor))
            {
                argVars.Add("--lightIntensity");
                argVars.Add(ExportConfig.LightIntensityFactor);
            }

            var extraArgs = string.Join(" ", [argFlags, string.Join(" ", argVars)]);

            ConsoleString = "";
            ConsoleString += $"ZoneFbx \"{GamePath}\" {argLevel} \"{argOutput}\\\" {extraArgs}\n";

            var finalArgs = new List<string>{ GamePath, argLevel, argOutput};

            if (!string.IsNullOrEmpty(argFlags))
            {
                finalArgs.Add(argFlags);
            }
            if (argVars.Count > 0)
            {
                finalArgs.AddRange(argVars);
            }

            try
            {
                var result = await Cli.Wrap("ZoneFbxCLI/ZoneFbx")
                    .WithArguments(finalArgs)
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
