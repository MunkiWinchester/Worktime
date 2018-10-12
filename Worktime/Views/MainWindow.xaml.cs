using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Shell;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro;
using Worktime.Business;
using Worktime.DataObjetcs;
using Worktime.Properties;
using Worktime.ViewModels;
using Worktime.Views.Tray;
using WpfUtility.Services;
using Application = System.Windows.Application;
using ToolTip = Worktime.Views.Tray.ToolTip;

namespace Worktime.Views
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Contains the view model
        /// </summary>
        private MainWindowViewModel _viewModel = new MainWindowViewModel();

        /// <summary>
        /// true if the timer reached the end, otherwise false
        /// </summary>
        private bool _endReached;

        private TaskbarIcon _taskbarIcon;

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        internal void LoadConfigSettings()
        {
            Top = Settings.Default.Top;
            Left = Settings.Default.Left;

            DataContext = _viewModel;
            _viewModel.ProgressChanged += _viewModel_ProgressChanged;
            _viewModel.RunningStateChanged += _viewModel_RunningStateChanged;

            _taskbarIcon = (TaskbarIcon)FindResource("TaskbarIcon");
            _taskbarIcon.DoubleClickCommand = new DelegateCommand(() => NotifyIconOnClick(false));
            if (_taskbarIcon.TrayPopup is Tray.ContextMenu contextMenu)
            {
                contextMenu.CloseCommand = new DelegateCommand(Close);
                contextMenu.MinimizeShowCommand = new DelegateCommand(() => NotifyIconOnClick(WindowState == WindowState.Normal));
                contextMenu.EditCommand = _viewModel.EditCommand;
                contextMenu.SettingsCommand = _viewModel.SettingsClickedCommand;
                contextMenu.AboutCommand = _viewModel.AboutClickedCommand;
                contextMenu.StartStopCommand = _viewModel.StartStopCommand;
            }

            Settings.Default.SelectedAccent = string.IsNullOrWhiteSpace(Settings.Default.SelectedAccent)
                ? "Crimson"
                : Settings.Default.SelectedAccent;
            Settings.Default.SelectedTheme = string.IsNullOrWhiteSpace(Settings.Default.SelectedTheme)
                ? "BaseDark"
                : Settings.Default.SelectedTheme;
            Settings.Default.Save();

            try
            {
                ThemeManager.ChangeAppStyle(Application.Current,
                    ThemeManager.GetAccent(Settings.Default.SelectedAccent),
                    ThemeManager.GetAppTheme(Settings.Default.SelectedTheme));
            }
            catch (Exception exception)
            {
                Logger.Error("LoadConfigSettings()", exception);
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Crimson"),
                    ThemeManager.GetAppTheme("BaseDark"));
            }

            _viewModel.InitControl();
        }

        /// <summary>
        /// Shows the window (from the system tray)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void NotifyIconOnClick(bool minimizeToTray)
        {
            var contextMenu = _taskbarIcon.TrayPopup as Tray.ContextMenu;

            if (!minimizeToTray)
            {
                // yes doubled entry, but it ain't stupid if it works
                WindowState = WindowState.Normal;
                Activate();
                Show();
                WindowState = WindowState.Normal;

                if(contextMenu != null)
                    contextMenu.IsMinimized = false;
            }
            else
            {
                Hide();
                WindowState = WindowState.Minimized;

                if (contextMenu != null)
                    contextMenu.IsMinimized = true;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Hides the window into the system tray
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (_taskbarIcon.TrayPopup is Tray.ContextMenu contextMenu)
                contextMenu.IsMinimized = WindowState == WindowState.Minimized;
            if (WindowState == WindowState.Minimized)
                Hide();
            base.OnStateChanged(e);
        }

        /// <summary>
        /// Occurs when the running state was changed
        /// </summary>
        /// <param name="running">The running state</param>
        private void _viewModel_RunningStateChanged(bool running)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (TaskbarItemInfo == null)
                    return;

                if (!_endReached)
                {
                    TaskbarItemInfo.ProgressState = running
                        ? TaskbarItemProgressState.Normal
                        : TaskbarItemProgressState.Paused;

                    if (_taskbarIcon.TrayToolTip is ToolTip trayToolTip)
                    {
                        trayToolTip.ProgressBarColor = running
                            ? (SolidColorBrush)Application.Current.FindResource("DayGreen")
                            : (SolidColorBrush)Application.Current.FindResource("PauseYellow");
                    }
                }

                if (_taskbarIcon.TrayPopup is Tray.ContextMenu contextMenu)
                    contextMenu.IsRunning = running;
            });
        }

        /// <summary>
        /// Occurs when the progress of the progress bar was changed
        /// </summary>
        /// <param name="percent">The percent value</param>
        /// <param name="notifyIconText"></param>
        private void _viewModel_ProgressChanged(Employee employee, double percent)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (TaskbarItemInfo == null)
                    return;

                var value = Math.Round(percent / 100, 2);
                if (percent >= 100)
                {
                    _endReached = true;
                    TaskbarItemInfo.ProgressValue = 1.0;
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                }
                else
                {
                    TaskbarItemInfo.ProgressValue = value;
                    TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                }

                if (_taskbarIcon.TrayToolTip is ToolTip trayToolTip)
                {
                    trayToolTip.Employee = employee;
                    trayToolTip.ProgressBarValue = percent;

                    trayToolTip.ProgressBarColor = _endReached
                        ? (SolidColorBrush)Application.Current.FindResource("DayRed")
                        : (SolidColorBrush)Application.Current.FindResource("DayGreen");
                }
            });
        }

        /// <summary>
        /// Saves the position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Settings.Default.Top = Top;
            Settings.Default.Left = Left;
            Settings.Default.Save();
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            var window = (Window) sender;
            window.Topmost = Settings.Default.IsAlwaysOnTop;
        }
    }
}