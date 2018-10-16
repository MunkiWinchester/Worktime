using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Worktime.DataObjetcs;

namespace Worktime.Views.Tray
{
    /// <summary>
    /// Interaction logic for TrayToolTip.xaml
    /// </summary>
    public partial class ToolTip : UserControl
    {
        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty WorkTimeProperty = DependencyProperty.Register(
            nameof(WorkTime), typeof(TimeSpan), typeof(ToolTip),
            new PropertyMetadata(new TimeSpan(0)));

        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty EstimatedCutProperty = DependencyProperty.Register(
            nameof(EstimatedCut), typeof(TimeSpan), typeof(ToolTip),
            new PropertyMetadata(new TimeSpan(0)));

        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty OvertimeProperty = DependencyProperty.Register(
            nameof(Overtime), typeof(TimeSpan), typeof(ToolTip),
            new PropertyMetadata(new TimeSpan(0)));

        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty ProgressBarColorProperty = DependencyProperty.Register(
            nameof(ProgressBarColor), typeof(SolidColorBrush), typeof(ToolTip),
            new PropertyMetadata((SolidColorBrush) Application.Current.FindResource("DayGreen")));

        /// <summary>
        /// DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty FilledProgressBarColorProperty = DependencyProperty.Register(
            nameof(FilledProgressBarColor), typeof(SolidColorBrush), typeof(ToolTip),
            new PropertyMetadata((SolidColorBrush) Application.Current.FindResource("DayRed")));

        /// <summary>
        /// DependencyProperty for the progress bar value
        /// </summary>
        public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register(
            nameof(ProgressBarValue), typeof(double), typeof(ToolTip), new PropertyMetadata(0d));

        /// <summary>
        /// Value of the progress bar color
        /// </summary>
        public TimeSpan WorkTime
        {
            get => (TimeSpan)GetValue(WorkTimeProperty);
            set => SetValue(WorkTimeProperty, value);
        }

        /// <summary>
        /// Value of the progress bar color
        /// </summary>
        public TimeSpan EstimatedCut
        {
            get => (TimeSpan)GetValue(EstimatedCutProperty);
            set => SetValue(EstimatedCutProperty, value);
        }

        /// <summary>
        /// Value of the progress bar color
        /// </summary>
        public TimeSpan Overtime
        {
            get => (TimeSpan)GetValue(OvertimeProperty);
            set => SetValue(OvertimeProperty, value);
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

        public ToolTip()
        {
            InitializeComponent();
        }
    }
}
