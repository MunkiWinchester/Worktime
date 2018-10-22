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
        private TimeSpan _breakTimeRegular;
        private IsoWeek _isoWeek;
        private ObservableCollection<Times> _times;
        private TimeSpan _weekWorkTimeRegular;
        private TimeSpan _workTimeRegular;
        private bool _isInitial = true;

        public Employee()
        {
            BreakTimeRegular = new TimeSpan(0, 30, 0);
            IsoWeek = new IsoWeek();
            Times = new ObservableCollection<Times>();
            WeekWorkTimeRegular = new TimeSpan(40, 0, 0);
            WorkTimeRegular = new TimeSpan(8, 0, 0);
            _isInitial = false;
        }

        [JsonConstructor]
        public Employee(TimeSpan breakTimeRegular, IsoWeek isoWeek, ObservableCollection<Times> times,
                        TimeSpan weekWorkTimeRegular, TimeSpan workTimeRegular)
        {
            BreakTimeRegular = breakTimeRegular;
            IsoWeek = isoWeek;
            Times = times;
            WeekWorkTimeRegular = weekWorkTimeRegular;
            WorkTimeRegular = workTimeRegular;
            _isInitial = false;
        }

        public IsoWeek IsoWeek
        {
            get => _isoWeek;
            set
            {
                if (SetField(ref _isoWeek, value))
                    ChangeHasChanges(this, nameof(IsoWeek));
            }
        }

        public ObservableCollection<Times> Times
        {
            get => _times;
            set
            {
                if (SetField(ref _times, value))
                {
                    ChangeHasChanges(this, nameof(Times));
                    OnPropertyChanged(nameof(Begin));
                    OnPropertyChanged(nameof(BreakTimeReal));
                    OnPropertyChanged(nameof(EstimatedCut));
                    OnPropertyChanged(nameof(Overtime));
                    OnPropertyChanged(nameof(WeekWorkTimeReal));
                    OnPropertyChanged(nameof(WorkTimeReal));

                    foreach (var time in _times)
                    {
                        time.OnChange +=ValueChanged;
                    }
                }
            }
        }

        private void ValueChanged(object sender, string e)
        {
            ChangeHasChanges(sender, e);
        }

        public TimeSpan WorkTimeRegular
        {
            get => _workTimeRegular;
            set
            {
                if (SetField(ref _workTimeRegular, value))
                {
                    ChangeHasChanges(this, nameof(WorkTimeRegular));
                    OnPropertyChanged(nameof(Overtime));
                }
            }
        }

        public TimeSpan BreakTimeRegular
        {
            get => _breakTimeRegular;
            set
            {
                if (SetField(ref _breakTimeRegular, value))
                {
                    ChangeHasChanges(this, nameof(BreakTimeRegular));
                    OnPropertyChanged(nameof(Overtime));
                }
            }
        }

        public TimeSpan WeekWorkTimeRegular
        {
            get => _weekWorkTimeRegular;
            set
            {
                if (SetField(ref _weekWorkTimeRegular, value))
                {
                    ChangeHasChanges(this, nameof(WeekWorkTimeRegular));
                    OnPropertyChanged(nameof(Overtime));
                }
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
            TimeSpan.FromSeconds(Times.Sum(t => t.SpanCorrected.TotalSeconds));
        #endregion [JsonIgnore]

        public override string ToString()
        {
            return $"{IsoWeek} - {WeekWorkTimeReal.ToFormatedString()} of {WeekWorkTimeRegular.ToFormatedString()}";
        }

        private void ChangeHasChanges(object sender, string e)
        {
            if (!_isInitial)
                HasChanges = true;
        }

        public void TrackChanges(bool activateTracking)
        {
            _isInitial = !activateTracking;
        }
    }
}