﻿using System;
using System.Linq;
using System.Timers;
using System.Windows.Input;
using Worktime.Business;
using Worktime.DataObjetcs;
using Worktime.Views;
using WpfUtility.Services;

namespace Worktime.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        // Events
        /// <summary>
        /// The delegate for the progress changed event
        /// </summary>
        /// <param name="percent">The percent value</param>
        public delegate void ProgressEvent(double percent, string notifyIconText);

        /// <summary>
        /// The delegate for the running state event
        /// </summary>
        /// <param name="running">true if the timer is running, otherwise false</param>
        public delegate void RunningStateEvent(bool running);

        private readonly Timer _timer30Min;
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
            _timer30Min = new Timer(TimeSpan.FromMinutes(30).TotalMilliseconds);
            _timer30Min.Elapsed += Timer30MinOnElapsed;
            // Activate those timers in the InitControl()
        }

        public bool StartEnabled
        {
            get => _startEnabled;
            set
            {
                _startEnabled = value;
                OnPropertyChanged();
            }
        }

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
        public ICommand StartCommand => new DelegateCommand(AddStamp);

        /// <summary>
        /// Command to refresh the values
        /// </summary>
        public ICommand StopCommand => new DelegateCommand(AddStamp);

        private void EditTimeFrames(MainWindow mainWindow)
        {
            var employeeValues = EmployeeManager.GetEmployeeValuesFromJson()?.OrderByDescending(ev => ev.IsoWeek).ToList();
            var editView = new EditWindow(mainWindow, employeeValues);
            var result = editView.ShowDialog();
            if (result.HasValue && result.Value)
            {
                foreach (var employee in employeeValues.Where(ev => ev.HasChanges))
                {
                    EmployeeManager.SaveEmployeeValues(employee);
                    if (employee.IsoWeek.Equals(Employee.IsoWeek))
                        Employee = employee;
                }
                RefreshView();
            }
        }

        private void AddStamp()
        {
            EmployeeManager.AddStamp(Employee);
            StartEnabled = StopEnabled;
            StopEnabled = !StopEnabled;
        }

        /// <summary>
        /// Occures when the 30 min timer ticks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="elapsedEventArgs"></param>
        private void Timer30MinOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            RefreshValues();
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
        public event ProgressEvent ProgressChanged;

        /// <summary>
        /// Occurs when the running state is changed
        /// </summary>
        public event RunningStateEvent RunningStateChanged;

        /// <summary>
        /// Inits the window
        /// </summary>
        public void InitControl()
        {
            RefreshValues();
            _timer30Sec.Enabled = _timer30Min.Enabled = true;
        }

        /// <summary>
        /// Refreshes the values
        /// </summary>
        private void RefreshValues()
        {
            RunningStateChanged?.Invoke(false);
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

                DayPercentageValue = Helpers.CalculatePercentage(employee.WorkTimeReal, employee.WorkTimeRegular);
                BreakPercentageValue = Helpers.CalculatePercentage(employee.BreakTimeReal, employee.BreakTimeRegular);
                WeekPercentageValue =
                    Helpers.CalculatePercentage(employee.WeekWorkTimeReal, employee.WeekWorkTimeRegular);

                RunningStateChanged?.Invoke(true);
                ProgressChanged?.Invoke(DayPercentageValue,
                    $@"Worktime!
Work time: {(int) employee.WorkTimeReal.TotalHours:00}:{employee.WorkTimeReal.Minutes:00} ({DayPercentageValue:0.##}%)
Est. cut: {(int) employee.EstimatedCut.TotalHours:00}:{employee.EstimatedCut.Minutes:00}");

                Employee = employee;
            }
        }

        /// <summary>
        /// Opens the settings window centered on the main window
        /// </summary>
        /// <param name="mainWindow"></param>
        private static void OpenSettings(MainWindow mainWindow)
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
            var settings = new AboutWindow(mainWindow);
            settings.ShowDialog();
        }
    }
}