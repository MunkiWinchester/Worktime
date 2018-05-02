using System.Windows;
using Worktime.DataObjetcs;

namespace Worktime.Views
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditView
    {
        public static readonly DependencyProperty EmployeeProperty = DependencyProperty.Register(
            nameof(Employee), typeof(Employee), typeof(EditView), new PropertyMetadata(default(Employee)));

        public Employee Employee
        {
            get => (Employee) GetValue(EmployeeProperty);
            set => SetValue(EmployeeProperty, value);
        }

        public EditView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Sets the DialogResult to true to close the window
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
