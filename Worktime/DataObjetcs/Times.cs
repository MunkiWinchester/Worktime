using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json;
using Worktime.Business;
using WpfUtility.Services;

namespace Worktime.DataObjetcs
{
    public class Times : ObservableObject
    {
        public event EventHandler<string> OnChange;
        private ObservableCollection<TimeFrame> _timeFrames;
        private DateTime _date;
        private readonly bool _isInitial = true;

        public Times()
        {
            TimeFrames = new ObservableCollection<TimeFrame>();
            _isInitial = false;
        }

        [JsonConstructor]
        public Times(DateTime date, ObservableCollection<TimeFrame> timeFrames)
        {
            Date = date;
            TimeFrames = timeFrames;
            _isInitial = false;
        }

        public ObservableCollection<TimeFrame> TimeFrames
        {
            get => _timeFrames;
            set
            {
                if (SetField(ref _timeFrames, value))
                {
                    if (!_isInitial)
                        OnChange?.Invoke(this, nameof(TimeFrames));
                    TriggerPropertiesOnChanged();

                    foreach (var timeFrame in _timeFrames)
                    {
                        timeFrame.OnChange += ValueChanged;
                    }
                }
            }
        }

        private void TriggerPropertiesOnChanged()
        {
            OnPropertyChanged(nameof(Span));
            OnPropertyChanged(nameof(SpanCorrected));
            OnPropertyChanged(nameof(SpanCorrectedExplanation));
            OnPropertyChanged(nameof(BreakTimeReal));
            OnPropertyChanged(nameof(BreakCalculated));
            OnPropertyChanged(nameof(BreakDifference));
        }

        private void ValueChanged(object sender, string e)
        {
            if (!_isInitial)
            {
                OnChange?.Invoke(this, nameof(TimeFrame));
                TriggerPropertiesOnChanged();
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (SetField(ref _date, value) && !_isInitial)
                    OnChange?.Invoke(this, nameof(Date));
            }
        }

        #region [JsonIgnore]
        [JsonIgnore]
        public TimeSpan Span =>
            TimeSpan.FromSeconds(TimeFrames.Where(t => t.Span != null).Sum(t => ((TimeSpan) t.Span).TotalSeconds));

        [JsonIgnore]
        public TimeSpan SpanCorrected => Span - BreakDifference;

        [JsonIgnore]
        public string SpanCorrectedExplanation
        {
            get
            {
                var result = $"{Span:hh\\:mm} - ";

                if (BreakTimeReal >= BreakCalculated)
                    result += $"({BreakTimeReal:hh\\:mm} - {BreakTimeReal:hh\\:mm})";
                else
                    result += $"({BreakCalculated:hh\\:mm} - {BreakTimeReal:hh\\:mm})";

                result += " = ";
                return result;
            }
        }

        [JsonIgnore]
        public TimeSpan BreakCalculated
        {
            get
            {
                if (Span.TotalHours > 9.75)
                    return new TimeSpan(0, 45, 0);
                return Span.TotalHours > 6.5 ?
                    new TimeSpan(0, 30, 0) :
                    new TimeSpan(0);
            }
        }

        [JsonIgnore]
        public TimeSpan BreakDifference
        {
            get
            {
                if (BreakTimeReal >= BreakCalculated)
                    return new TimeSpan(0);
                else
                    return BreakCalculated - BreakTimeReal;
            }
        }

        [JsonIgnore]
        public TimeSpan BreakTimeReal
        {
            get
            {
                if (TimeFrames != null && TimeFrames.Any())
                {
                    var span = new TimeSpan();
                    for (var i = 0; i + 1 < TimeFrames.Count; i++)
                    {
                        var first = TimeFrames[i];
                        var second = TimeFrames[i + 1];

                        if (first.End != null)
                            span += second.Begin - (TimeSpan) first.End;
                    }

                    return span;
                }

                return new TimeSpan(0);
            }
        }

        [JsonIgnore]
        public ICommand AddTimeFrameCommand => new DelegateCommand(AddTimeFrame);

        [JsonIgnore]
        public ICommand RemoveTimeFrameCommand => new RelayCommand<TimeFrame>(RemoveTimeFrame);
        #endregion [JsonIgnore]

        private void RemoveTimeFrame(TimeFrame timeFrame)
        {
            var timeFrames = TimeFrames;
            timeFrames.Remove(timeFrame);
            if (timeFrame.IsCurrent)
            {
                var frame = timeFrames.OrderByDescending(x => x.Begin).FirstOrDefault();
                if (frame != null)
                    frame.IsCurrent = true;
            }
            TimeFrames = new ObservableCollection<TimeFrame>(timeFrames);
        }

        private void AddTimeFrame()
        {
            var timeFrames = TimeFrames;
            var newFrame = new TimeFrame();
            if (Date.DayOfYear == DateTime.Now.DayOfYear)
            {
                var frame = timeFrames.FirstOrDefault(x => x.IsCurrent);
                if(frame != null)
                    frame.IsCurrent = false;
                newFrame.IsCurrent = true;
            }
            timeFrames.Add(newFrame);
            TimeFrames = new ObservableCollection<TimeFrame>(timeFrames);
        }

        public override string ToString()
        {
            return $"{Date:dd\\.MM\\.} --- {Span:hh\\:mm}h";
        }
    }
}