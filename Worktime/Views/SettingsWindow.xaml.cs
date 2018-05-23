using System.Windows;

namespace Worktime.Views
{
    /// <inheritdoc cref="System.Windows.Controls.UserControl" />
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance
        /// </summary>
        private SettingsWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="owner">Owner of the window</param>
        public SettingsWindow(Window owner) : this()
        {
            Owner = owner;
        }

        /// <summary>
        /// Sets the DialogResult to true to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}