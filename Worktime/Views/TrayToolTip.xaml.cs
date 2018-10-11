﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Worktime.DataObjetcs;

namespace Worktime.Views
{
    /// <summary>
    /// Interaction logic for TrayToolTip.xaml
    /// </summary>
    public partial class TrayToolTip : UserControl
    {
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty EmployeeProperty = DependencyProperty.Register(
            nameof(Employee), typeof(Employee), typeof(TrayToolTip),
            new PropertyMetadata(new Employee()));

        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty ProgressBarColorProperty = DependencyProperty.Register(
            nameof(ProgressBarColor), typeof(SolidColorBrush), typeof(TrayToolTip),
            new PropertyMetadata((SolidColorBrush) Application.Current.FindResource("DayGreen")));

        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty FilledProgressBarColorProperty = DependencyProperty.Register(
            nameof(FilledProgressBarColor), typeof(SolidColorBrush), typeof(TrayToolTip),
            new PropertyMetadata((SolidColorBrush) Application.Current.FindResource("DayRed")));

        /// <summary>
        /// DependencyProperty for the progress bar value
        /// </summary>
        public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register(
            nameof(ProgressBarValue), typeof(double), typeof(TrayToolTip), new PropertyMetadata(0d));

        /// <summary>
        /// Value of the top label
        /// </summary>
        public Employee Employee
        {
            get => (Employee)GetValue(EmployeeProperty);
            set => SetValue(EmployeeProperty, value);
        }

        /// <summary>
        /// Value of the progress bar color
        /// </summary>
        public SolidColorBrush ProgressBarColor
        {
            get => (SolidColorBrush)GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }

        /// <summary>
        /// Value of the progress bar value
        /// </summary>
        public double ProgressBarValue
        {
            get
            {
                var value = GetValue(ProgressBarValueProperty);
                if (value != null) return (double)value;
                return 0;
            }
            set => SetValue(ProgressBarValueProperty, value);
        }

        /// <summary>
        /// Value of the filled progress bar color
        /// </summary>
        public SolidColorBrush FilledProgressBarColor
        {
            get => (SolidColorBrush)GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }

        public TrayToolTip()
        {
            InitializeComponent();
        }
    }
}