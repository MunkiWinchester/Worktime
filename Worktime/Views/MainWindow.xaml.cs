using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using Hardcodet.Wpf.TaskbarNotification;
using MahApps.Metro;
using Worktime.Business;
using Worktime.DataObjetcs;
using Worktime.ViewModels;
using WpfUtility.Services;
using Application = System.Windows.Application;
using Settings = Worktime.Business.Settings;

namespace Worktime.Views
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IDisposable
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

            _taskbarIcon = (TaskbarIcon)FindResource("TaskbarIcon");
            _taskbarIcon.TrayPopup = new Tray.ContextMenu();
            _taskbarIcon.TrayToolTip = new Tray.ToolTip();
        }

        internal void LoadConfigSettings()
        {
            Top = Settings.Default.Top;
            Left = Settings.Default.Left;

            DataContext = _viewModel;
            _viewModel.ProgressChanged += _viewModel_ProgressChanged;
            _viewModel.RunningStateChanged += _viewModel_RunningStateChanged;
            Update.Updater.StatusBar.PropertyChanged += StatusBar_PropertyChanged;
            UiStyleManager.IsStyleChanged += UiStyleManager_IsStyleChanged;

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
            Settings.Save();

            try
            {
                UiStyleManager.ChangeAppStyle(Settings.Default.SelectedAccent, Settings.Default.SelectedTheme);
            }
            catch (Exception exception)
            {
                Logger.Error("LoadConfigSettings()", exception);
                UiStyleManager.ChangeAppStyle("Crimson", "BaseDark");
            }

            _viewModel.InitControl();
        }

        private void UiStyleManager_IsStyleChanged(object sender, StyleChangeEventArgs e)
        {
            var accentColor = e.Accent.Resources["AccentColorBrush"] as SolidColorBrush;
            if (_taskbarIcon.TrayToolTip is Tray.ToolTip toolTip)
                toolTip.AccentColor = accentColor;
            if (_taskbarIcon.TrayPopup is Tray.ContextMenu contextMenu)
                contextMenu.AccentColor = accentColor;
        }

        private void StatusBar_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(sender is Update.StatusBarHelper statusBar && statusBar.Visibility == Visibility.Visible)
                _taskbarIcon.ShowBalloonTip("New Update Available!", $@"There is a new update available.{
                    Environment.NewLine}Please restart the application.", BalloonIcon.None);
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
        private void _viewModel_RunningStateChanged(object sender, RunningStateEventArgs eventArgs)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (TaskbarItemInfo == null)
                    return;

                if (!_endReached)
                {
                    TaskbarItemInfo.ProgressState = eventArgs.IsRunning
                        ? TaskbarItemProgressState.Normal
                        : TaskbarItemProgressState.Paused;

                    if (_taskbarIcon.TrayToolTip is Tray.ToolTip trayToolTip)
                    {
                        trayToolTip.ProgressBarColor = eventArgs.IsRunning
                            ? (SolidColorBrush)Application.Current.FindResource("DayGreen")
                            : (SolidColorBrush)Application.Current.FindResource("PauseYellow");
                    }
                }

                if (_taskbarIcon.TrayPopup is Tray.ContextMenu contextMenu)
                    contextMenu.IsRunning = eventArgs.IsRunning;
            });
        }

        /// <summary>
        /// Occurs when the progress of the progress bar was changed
        /// </summary>
        /// <param name="percent">The percent value</param>
        /// <param name="notifyIconText"></param>
        private void _viewModel_ProgressChanged(object sender, ProgressEventArgs eventArgs)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                if (TaskbarItemInfo == null)
                    return;

                var value = Math.Round(eventArgs.Percent / 100, 2);
                if (eventArgs.Percent >= 100)
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

                if (_taskbarIcon.TrayToolTip is Tray.ToolTip trayToolTip)
                {
                    trayToolTip.WorkTime = eventArgs.Employee.WorkTimeReal;
                    trayToolTip.EstimatedCut = eventArgs.Employee.EstimatedCut;
                    trayToolTip.Overtime = eventArgs.Employee.Overtime;
                    trayToolTip.ProgressBarValue = eventArgs.Percent;

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
            Settings.Save();
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            var window = (Window) sender;
            window.Topmost = Settings.Default.IsAlwaysOnTop;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _viewModel.Dispose();
        }
    }
}