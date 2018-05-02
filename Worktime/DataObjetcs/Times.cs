using System;
using System.Collections.ObjectModel;
using System.Linq;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using WpfUtility;

namespace Worktime.DataObjetcs
{
    public class Times : ObservableObject
    {
        private ObservableCollection<TimeFrame> _timeFrames = new ObservableCollection<TimeFrame>();
        private DateTime _date;

        public ObservableCollection<TimeFrame> TimeFrames
        {
            get => _timeFrames;
            set
            {
                _timeFrames = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public TimeSpan Span => TimeSpan.FromSeconds(TimeFrames.Sum(t => t.Span.TotalSeconds));

        [JsonIgnore]
        public TimeSpan SpanCorrected => Span - BreakDifference;

        [JsonIgnore]
        public string SpanCorrectedExplanation =>
            $@"{Span:hh\:mm} - ( | {BreakCalculated:hh\:mm}{
                    (BreakDifference.TotalMinutes.IsZero()
                        ? " - 00:30"
                        : $" - {BreakTimeReal:hh\\:mm}")
                } | ) =";

        [JsonIgnore]
        public TimeSpan BreakCalculated
        {
            get
            {
                if (Span.TotalHours > 9) return new TimeSpan(0, 45, 0);
                return Span.TotalHours > 6 ? new TimeSpan(0, 30, 0) : new TimeSpan(0);
            }
        }

        [JsonIgnore]
        public TimeSpan BreakDifference =>
            BreakTimeReal > BreakCalculated ? 
                BreakTimeReal - BreakCalculated : 
                BreakCalculated - BreakTimeReal;

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

                        if (first.End != null) span += second.Begin - (DateTime) first.End;
                    }

                    return span;
                }

                return new TimeSpan(0);
            }
        }

        public override string ToString()
        {
            return $"{Date:dd.MM.} --- {Span:hh:mm}h";
        }
    }
}