using System.Collections.Generic;
using System.Linq;
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

        public static readonly DependencyProperty EmployeeValuesProperty = DependencyProperty.Register(
            nameof(EmployeeValues), typeof(List<Employee>), typeof(EditWindow), new PropertyMetadata(default(List<Employee>)));

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
        public EditWindow(Window owner, List<Employee> employeeValues) : this()
        {
            Owner = owner;
            EmployeeValues = employeeValues;
            if (EmployeeValues.Any())
                Employee = EmployeeValues.FirstOrDefault();
        }

        public Employee Employee
        {
            get => (Employee)GetValue(EmployeeProperty);
            set => SetValue(EmployeeProperty, value);
        }

        public List<Employee> EmployeeValues
        {
            get => (List<Employee>)GetValue(EmployeeValuesProperty);
            set => SetValue(EmployeeValuesProperty, value);
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

        private void ButtonMaximize_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState =
                WindowState == WindowState.Maximized ?
                    WindowState.Normal :
                    WindowState.Maximized;
        }
    }
}