using System.Windows;
using System.Windows.Media;

namespace Worktime.Views
{
    /// <summary>
    ///     Interaction logic for ProgressbarView.xaml
    /// </summary>
    public partial class ProgressbarView
    {
        /// <summary>
        ///     DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty ProgressBarColorProperty = DependencyProperty.Register(
            nameof(ProgressBarColor), typeof(SolidColorBrush), typeof(ProgressbarView),
            new PropertyMetadata(new SolidColorBrush(Colors.LimeGreen)));

        /// <summary>
        ///     DependencyProperty for the progress bar color
        /// </summary>
        public static readonly DependencyProperty FilledProgressBarColorProperty = DependencyProperty.Register(
            nameof(FilledProgressBarColor), typeof(SolidColorBrush), typeof(ProgressbarView),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        /// <summary>
        ///     DependencyProperty for the progress bar value
        /// </summary>
        public static readonly DependencyProperty ProgressBarValueProperty = DependencyProperty.Register(
            nameof(ProgressBarValue), typeof(double), typeof(ProgressbarView), new PropertyMetadata(0d));

        /// <summary>
        ///     DependencyProperty for the top label
        /// </summary>
        public static readonly DependencyProperty MinimumLabelProperty = DependencyProperty.Register(
            nameof(MinimumLabel), typeof(string), typeof(ProgressbarView), new PropertyMetadata(""));

        /// <summary>
        ///     DependencyProperty for the bottom label
        /// </summary>
        public static readonly DependencyProperty MaximumLabelProperty = DependencyProperty.Register(
            nameof(MaximumLabel), typeof(string), typeof(ProgressbarView), new PropertyMetadata(""));

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public ProgressbarView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///     Value of the progress bar color
        /// </summary>
        public SolidColorBrush ProgressBarColor
        {
            get => (SolidColorBrush) GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }

        /// <summary>
        ///     Value of the progress bar value
        /// </summary>
        public double ProgressBarValue
        {
            get
            {
                var value = GetValue(ProgressBarValueProperty);
                if (value != null) return (double) value;
                return 0;
            }
            set => SetValue(ProgressBarValueProperty, value);
        }

        /// <summary>
        ///     Value of the top label
        /// </summary>
        public string MinimumLabel
        {
            get => (string) GetValue(MinimumLabelProperty);
            set => SetValue(MinimumLabelProperty, value);
        }

        /// <summary>
        ///     Value of the bottom label
        /// </summary>
        public string MaximumLabel
        {
            get => (string) GetValue(MaximumLabelProperty);
            set => SetValue(MaximumLabelProperty, value);
        }

        public SolidColorBrush FilledProgressBarColor
        {
            get => (SolidColorBrush)GetValue(ProgressBarColorProperty);
            set => SetValue(ProgressBarColorProperty, value);
        }
    }
}