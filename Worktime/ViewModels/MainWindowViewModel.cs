using System;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Worktime.Business;
using Worktime.DataObjetcs;
using Worktime.Views;
using WpfUtility.Services;

namespace Worktime.ViewModels
{
    public class MainWindowViewModel : ObservableObject, IDisposable
    {
        private readonly Timer _timer30Sec;
        private double _breakPercentageValue;
        private double _dayPercentageValue;
        private Employee _employee;
        private bool _startEnabled = true;
        private bool _stopEnabled;
        private double _weekPercentageValue;

        /// <summary>
        /// Initializes a main window view model
        /// </summary>
        public MainWindowViewModel()
        {
            _employee = new Employee();

            _timer30Sec = new Timer(TimeSpan.FromSeconds(30).TotalMilliseconds);
            _timer30Sec.Elapsed += Timer30SecOnElapsed;
            // Activate those timers in the InitControl()
        }

        /// <summary>
        /// Property to define if the start button is enabled (true ^= not running)
        /// </summary>
        public bool StartEnabled
        {
            get => _startEnabled;
            set
            {
                _startEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Property to define if the stop button is enabled (true ^= running)
        /// </summary>
        public bool StopEnabled
        {
            get => _stopEnabled;
            set
            {
                _stopEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Employee
        /// </summary>
        public Employee Employee
        {
            get => _employee;
            set
            {
                _employee = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command to open the settings
        /// </summary>
        public ICommand SettingsClickedCommand => new RelayCommand<MainWindow>(OpenSettings);

        /// <summary>
        /// Command to open the edit view
        /// </summary>
        public ICommand EditCommand => new RelayCommand<MainWindow>(EditTimeFrames);

        /// <summary>
        /// Command to open the about view
        /// </summary>
        public ICommand AboutClickedCommand => new RelayCommand<MainWindow>(OpenAbout);

        /// <summary>
        /// Percentage value of the day done
        /// </summary>
        public double DayPercentageValue
        {
            get => _dayPercentageValue;
            set
            {
                _dayPercentageValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage value of the break done
        /// </summary>
        public double BreakPercentageValue
        {
            get => _breakPercentageValue;
            set
            {
                _breakPercentageValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage value of the week done
        /// </summary>
        public double WeekPercentageValue
        {
            get => _weekPercentageValue;
            set
            {
                _weekPercentageValue = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Command to refresh the values
        /// </summary>
        public ICommand StartStopCommand => new DelegateCommand(AddStamp);

        public ICommand UpdateCommand => new DelegateCommand(Update.Updater.StartUpdate);

        private void EditTimeFrames(MainWindow mainWindow)
        {
            var employeeValues = EmployeeManager.GetEmployeeValuesFromJson()?.OrderByDescending(ev => ev.IsoWeek).ToList();
            var editView = new EditWindow(mainWindow, employeeValues);
            var result = editView.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var (currentHasChanged, currentEmployee)= EmployeeManager.SaveEmployeeValues(employeeValues, Employee.IsoWeek);
                if (currentHasChanged)
                {
                    Employee = currentEmployee;
                }
                RefreshView();
            }
        }

        private void AddStamp()
        {
            EmployeeManager.AddStamp(Employee);
            StartEnabled = StopEnabled;
            StopEnabled = !StopEnabled;
            RunningStateChanged?.Invoke(this, new RunningStateEventArgs(StopEnabled));
        }

        /// <summary>
        /// Occures when the 1 sec timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private void Timer30SecOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            RefreshView();
        }

        /// <summary>
        /// Occurs when the progress of the progress bar changed
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        /// <summary>
        /// Occurs when the running state is changed
        /// </summary>
        public event EventHandler<RunningStateEventArgs> RunningStateChanged;

        /// <summary>
        /// Inits the window
        /// </summary>
        public void InitControl()
        {
            RefreshValues();
            _timer30Sec.Enabled = true;
        }

        /// <summary>
        /// Refreshes the values
        /// </summary>
        private void RefreshValues()
        {
            var employee = EmployeeManager.LoadEmployeeValues();

            Employee = employee;

            if (employee.Times.Where(t => t.Date.DayOfYear == DateTime.Now.DayOfYear)
                .Any(tf => tf.TimeFrames.Any(t => t.End == null)))
            {
                StartEnabled = false;
                StopEnabled = true;
            }

            RefreshView();
        }

        /// <summary>
        /// Refreshes the view
        /// </summary>
        private void RefreshView()
        {
            if (Employee != null)
            {
                var employee = Employee;

                DayPercentageValue = Helper.CalculatePercentage(employee.WorkTimeReal, employee.WorkTimeRegular);
                BreakPercentageValue = Helper.CalculatePercentage(employee.BreakTimeReal, employee.BreakTimeRegular);
                WeekPercentageValue =
                    Helper.CalculatePercentage(employee.WeekWorkTimeReal, employee.WeekWorkTimeRegular);

                ProgressChanged?.Invoke(this, new ProgressEventArgs(employee, DayPercentageValue));
                RunningStateChanged?.Invoke(this, new RunningStateEventArgs(StopEnabled));

                Employee = employee;
            }
        }

        /// <summary>
        /// Opens the settings window centered on the main window
        /// </summary>
        /// <param name="mainWindow"></param>
        private void OpenSettings(MainWindow mainWindow)
        {
            var settings = new SettingsWindow(mainWindow);
            settings.ShowDialog();
        }

        /// <summary>
        /// Opens the about window centered on the main window
        /// </summary>
        /// <param name="mainWindow"></param>
        private static void OpenAbout(MainWindow mainWindow)
        {
            var about = new AboutWindow(mainWindow);
            about.ShowDialog();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _timer30Sec.Dispose();
        }
    }
}