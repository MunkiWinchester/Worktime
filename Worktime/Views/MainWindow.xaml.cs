using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Shell;
using MahApps.Metro;
using Worktime.Properties;
using Worktime.ViewModels;
using Application = System.Windows.Application;

namespace Worktime.Views
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Contains the notify icon
        /// </summary>
        private NotifyIcon _notifyIcon;

        /// <summary>
        /// Contains the view model
        /// </summary>
        private MainWindowViewModel _viewModel = new MainWindowViewModel();

        /// <summary>
        /// true if the timer reached the end, otherwise false
        /// </summary>
        private bool _endReached;

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

            _notifyIcon =
                new NotifyIcon
                {
                    Icon = Properties.Resources.Utilities_clock,
                    Visible = true,
                    Text = @"Worktime!"
                };
            _notifyIcon.Click += NotifyIconOnClick;

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
                Console.WriteLine(exception);
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
        private void NotifyIconOnClick(object sender, EventArgs eventArgs)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        /// <inheritdoc />
        /// <summary>
        /// Hides the window into the system tray
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
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
                    TaskbarItemInfo.ProgressState = running
                        ? TaskbarItemProgressState.Normal
                        : TaskbarItemProgressState.Paused;
            });
        }

        /// <summary>
        /// Occurs when the progress of the progress bar was changed
        /// </summary>
        /// <param name="percent">The percent value</param>
        /// <param name="notifyIconText"></param>
        private void _viewModel_ProgressChanged(double percent, string notifyIconText)
        {
            _notifyIcon.Text = notifyIconText;
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