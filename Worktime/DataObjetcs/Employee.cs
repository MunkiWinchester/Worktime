using System;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;
using Worktime.Business;
using Worktime.Extension;
using WpfUtility.Services;

namespace Worktime.DataObjetcs
{
    public class Employee : ObservableObject
    {
        private TimeSpan _breakTimeRegular = new TimeSpan(0, 30, 0);
        private IsoWeek _isoWeek = new IsoWeek();
        private ObservableCollection<Times> _times = new ObservableCollection<Times>();
        private TimeSpan _weekWorkTimeRegular = new TimeSpan(40, 0, 0);
        private TimeSpan _workTimeRegular = new TimeSpan(8, 0, 0);

        public IsoWeek IsoWeek
        {
            get => _isoWeek;
            set
            {
                _isoWeek = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Times> Times
        {
            get => _times;
            set
            {
                SetField(ref _times, value);
                OnPropertyChanged(nameof(Begin));
                OnPropertyChanged(nameof(BreakTimeReal));
                OnPropertyChanged(nameof(EstimatedCut));
                OnPropertyChanged(nameof(Overtime));
                OnPropertyChanged(nameof(WeekWorkTimeReal));
                OnPropertyChanged(nameof(WorkTimeReal));
            }
        }

        public TimeSpan WorkTimeRegular
        {
            get => _workTimeRegular;
            set
            {
                _workTimeRegular = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Overtime));
            }
        }

        public TimeSpan BreakTimeRegular
        {
            get => _breakTimeRegular;
            set
            {
                _breakTimeRegular = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Overtime));
            }
        }

        public TimeSpan WeekWorkTimeRegular
        {
            get => _weekWorkTimeRegular;
            set
            {
                _weekWorkTimeRegular = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Overtime));
            }
        }

        #region [JsonIgnore]
        [JsonIgnore]
        public bool HasChanges { get; set; }

        [JsonIgnore]
        public TimeSpan Begin
        {
            get
            {
                var timeFrame = Times.FirstOrDefault(t => t.Date.DayOfYear == DateTime.Now.DayOfYear)?.TimeFrames
                    .OrderBy(t => t.Begin)
                    .FirstOrDefault();
                // ReSharper disable once MergeConditionalExpression
                return timeFrame?.Begin != null ? timeFrame.Begin : new TimeSpan();
            }
        }

        [JsonIgnore]
        public TimeSpan Overtime => EmployeeManager.CalculateTotalOvertime(this);

        [JsonIgnore]
        public TimeSpan EstimatedCut =>
            Begin + WorkTimeRegular +
            (BreakTimeReal > BreakTimeRegular ? BreakTimeReal : BreakTimeRegular);

        [JsonIgnore]
        public TimeSpan WorkTimeReal =>
            Times.FirstOrDefault(t => t.Date.DayOfYear == DateTime.Now.DayOfYear)?.SpanCorrected ?? new TimeSpan();

        [JsonIgnore]
        public TimeSpan BreakTimeReal
        {
            get
            {
                var todaysFrames =
                    Times.FirstOrDefault(t => t.Date.DayOfYear == DateTime.Now.DayOfYear)?.TimeFrames.ToList();
                if (todaysFrames != null && todaysFrames.Any())
                {
                    var span = new TimeSpan();
                    for (var i = 0; i + 1 < todaysFrames.Count; i++)
                    {
                        var first = todaysFrames[i];
                        var second = todaysFrames[i + 1];

                        if (first.End != null) span += second.Begin - (TimeSpan)first.End;
                    }

                    return span;
                }

                return new TimeSpan(0);
            }
        }

        [JsonIgnore]
        public TimeSpan WeekWorkTimeReal =>
            TimeSpan.FromSeconds(Times.Sum(t => t.SpanCorrected.TotalSeconds)); // TODO: Pausenzeiten beachten!
        #endregion [JsonIgnore]

        public override string ToString()
        {
            return $"{IsoWeek} - {WeekWorkTimeReal.ToFormatedString()} of {WeekWorkTimeRegular.ToFormatedString()}";
        }
    }
}