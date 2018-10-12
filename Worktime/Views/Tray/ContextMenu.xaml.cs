using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfUtility.Services;

namespace Worktime.Views.Tray
{
    /// <summary>
    /// Interaction logic for TrayToolTip.xaml
    /// </summary>
    public partial class ContextMenu : UserControl
    {
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty EditCommandProperty = DependencyProperty.Register(
            nameof(EditCommand), typeof(ICommand), typeof(ContextMenu),
            new PropertyMetadata(new DelegateCommand(() => { })));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty SettingsCommandProperty = DependencyProperty.Register(
            nameof(SettingsCommand), typeof(ICommand), typeof(ContextMenu),
            new PropertyMetadata(new DelegateCommand(() => { })));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty MinimizeShowCommandProperty = DependencyProperty.Register(
            nameof(MinimizeShowCommand), typeof(ICommand), typeof(ContextMenu),
            new PropertyMetadata(new DelegateCommand(() => { })));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
            nameof(CloseCommand), typeof(ICommand), typeof(ContextMenu),
            new PropertyMetadata(new DelegateCommand(() => { })));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty AboutCommandProperty = DependencyProperty.Register(
            nameof(AboutCommand), typeof(ICommand), typeof(ContextMenu),
            new PropertyMetadata(new DelegateCommand(() => { })));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty StartStopCommandProperty = DependencyProperty.Register(
            nameof(StartStopCommand), typeof(ICommand), typeof(ContextMenu),
            new PropertyMetadata(new DelegateCommand(() => { })));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
            nameof(IsRunning), typeof(bool), typeof(ContextMenu),
            new PropertyMetadata(true));
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register(
            nameof(IsMinimized), typeof(bool), typeof(ContextMenu),
            new PropertyMetadata(false));

        /// <summary>
        /// Value of the top label
        /// </summary>
        public ICommand EditCommand
        {
            get => (ICommand)GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public ICommand SettingsCommand
        {
            get => (ICommand)GetValue(SettingsCommandProperty);
            set => SetValue(SettingsCommandProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public ICommand MinimizeShowCommand
        {
            get => (ICommand)GetValue(MinimizeShowCommandProperty);
            set => SetValue(MinimizeShowCommandProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public ICommand AboutCommand
        {
            get => (ICommand)GetValue(AboutCommandProperty);
            set => SetValue(AboutCommandProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public ICommand StartStopCommand
        {
            get => (ICommand)GetValue(StartStopCommandProperty);
            set => SetValue(StartStopCommandProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public bool IsRunning
        {
            get => (bool)GetValue(IsRunningProperty);
            set => SetValue(IsRunningProperty, value);
        }

        /// <summary>
        /// Value of the top label
        /// </summary>
        public bool IsMinimized
        {
            get => (bool)GetValue(IsMinimizedProperty);
            set => SetValue(IsMinimizedProperty, value);
        }

        public ContextMenu()
        {
            InitializeComponent();
        }
    }
}
