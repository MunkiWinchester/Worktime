using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using PublicHoliday;
using Worktime.Business;
using Worktime.DataObjects;
using WpfUtility.Services;

namespace Worktime.ViewModels
{
    public class SettingsWindowViewModel : ObservableObject
    {
        private readonly bool _initial;

        private ObservableCollection<string> _accents;

        private bool _isAlwaysOnTop;

        private string _selectedAccent;

        /// <summary>
        /// Property for the selected german state
        /// </summary>
        private GermanPublicHoliday.States _selectedState;

        private string _selectedTheme;

        /// <summary>
        /// Property for the differen states of germany
        /// </summary>
        private ObservableCollection<GermanPublicHoliday.States> _states;

        private ObservableCollection<string> _themes;

        /// <summary>
        /// Initializes a settings window view model
        /// </summary>
        public SettingsWindowViewModel()
        {
            Themes = new ObservableCollection<string>(ThemeManager.AppThemes.Select(x => x.Name));
            Accents = new ObservableCollection<string>(ThemeManager.Accents.Select(x => x.Name));
            States = new ObservableCollection<GermanPublicHoliday.States>(
                ((GermanPublicHoliday.States[]) Enum.GetValues(typeof(GermanPublicHoliday.States))).Where(x =>
                    x != GermanPublicHoliday.States.ALL));
            _initial = true;
            SelectedAccent = Settings.Default.SelectedAccent;
            SelectedTheme = Settings.Default.SelectedTheme;
            IsAlwaysOnTop = Settings.Default.IsAlwaysOnTop;
            SelectedState = States.FirstOrDefault(x => x.ToString().Equals(Settings.Default.SelectedState));
            _initial = false;
        }

        /// <summary>
        /// Property for the selected german state
        /// </summary>
        public GermanPublicHoliday.States SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Property for the differen states of germany
        /// </summary>
        public ObservableCollection<GermanPublicHoliday.States> States
        {
            get => _states;
            set => SetField(ref _states, value);
        }

        /// <summary>
        /// Command to save the settings
        /// </summary>
        public ICommand SaveCommand => new DelegateCommand(SaveSettings);

        /// <summary>
        /// Command to cancle the settings
        /// </summary>
        public ICommand CancelCommand => new DelegateCommand(ResetSettings);

        /// <summary>
        /// Command to switch the style of the app
        /// </summary>
        public ICommand SwitchCommand => new DelegateCommand(SwitchAppStyle);

        /// <summary>
        /// Collection with the different accents
        /// </summary>
        public ObservableCollection<string> Accents
        {
            get => _accents;
            set => SetField(ref _accents, value);
        }

        /// <summary>
        /// Collection with the different themes
        /// </summary>
        public ObservableCollection<string> Themes
        {
            get => _themes;
            set => SetField(ref _themes, value);
        }

        /// <summary>
        /// Value of the selected accent
        /// </summary>
        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                _selectedAccent = value;
                SwitchAppStyle();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Value of the selected theme
        /// </summary>
        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                SwitchAppStyle();
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Value if the app should always be on top
        /// </summary>
        public bool IsAlwaysOnTop
        {
            get => _isAlwaysOnTop;
            set
            {
                _isAlwaysOnTop = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Switches the theme and/or accent of the app
        /// </summary>
        private void SwitchAppStyle()
        {
            if (!_initial)
            {
                if (SelectedAccent == UiTheme.WindowAccentName)
                    UiTheme.CreateWindowsAccentStyle(true, ThemeManager.GetAppTheme(_selectedTheme));
                else
                    ThemeManager.ChangeAppStyle(Application.Current,
                        ThemeManager.GetAccent(_selectedAccent),
                        ThemeManager.GetAppTheme(_selectedTheme));
            }
        }

        /// <summary>
        /// Resets the settings to the previous ones
        /// </summary>
        private static void ResetSettings()
        {
            ThemeManager.ChangeAppStyle(Application.Current,
                ThemeManager.GetAccent(Settings.Default.SelectedAccent),
                ThemeManager.GetAppTheme(Settings.Default.SelectedTheme));
        }

        /// <summary>
        /// Saves the selected accent and theme to the settings
        /// </summary>
        private void SaveSettings()
        {
            Settings.Default.SelectedAccent = SelectedAccent;
            Settings.Default.SelectedTheme = SelectedTheme;
            Settings.Default.IsAlwaysOnTop = IsAlwaysOnTop;
            Settings.Default.SelectedState = SelectedState.ToString();
            Settings.Save();
        }
    }
}