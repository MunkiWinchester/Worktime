using System.Windows;
using Worktime.DataObjetcs;

namespace Worktime.Views
{
    /// <inheritdoc cref="System.Windows.Controls.UserControl" />
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditWindow
    {
        public static readonly DependencyProperty EmployeeProperty = DependencyProperty.Register(
            nameof(Employee), typeof(Employee), typeof(EditWindow), new PropertyMetadata(default(Employee)));

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance
        /// </summary>
        private EditWindow()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="owner">Owner of the window</param>
        /// <param name="employee"></param>
        public EditWindow(Window owner, Employee employee) : this()
        {
            Owner = owner;
            Employee = employee;
        }

        public Employee Employee
        {
            get => (Employee) GetValue(EmployeeProperty);
            set => SetValue(EmployeeProperty, value);
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

        private void ButtonCancle_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}