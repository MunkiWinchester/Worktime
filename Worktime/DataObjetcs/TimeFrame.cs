using System;
using Newtonsoft.Json;
using Worktime.Extension;
using WpfUtility.Services;

namespace Worktime.DataObjetcs
{
    public class TimeFrame : ObservableObject
    {
        private TimeSpan _begin;
        private TimeSpan? _end;
        private bool _isCurrent;

        public TimeFrame()
        {
            Begin = DateTime.Now.ToDatelessTimeSpan();
        }

        public bool IsCurrent
        {
            get => _isCurrent;
            set
            {
                _isCurrent = value;
                OnPropertyChanged();
            }
        }

        public TimeSpan Begin
        {
            get => _begin;
            set
            {
                _begin = new TimeSpan(value.Hours, value.Minutes, value.Seconds);
                OnPropertyChanged();
                OnPropertyChanged(nameof(Span));
            }
        }

        public TimeSpan? End
        {
            get => _end;
            set
            {
                if (value is TimeSpan notNullValue)
                    _end = new TimeSpan(notNullValue.Hours, notNullValue.Minutes, notNullValue.Seconds);
                else
                    _end = null;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Span));
            }
        }

        [JsonIgnore]
        public TimeSpan? Span
        {
            get
            {
                if (End == null && IsCurrent)
                    return DateTime.Now.ToDatelessTimeSpan() - Begin;
                if (End != null)
                    return End - Begin;
                return null;
            }
        }

        public override string ToString()
        {
            return $"{Begin:dd.MM. HH:mm} --- {End:dd.MM. HH:mm}";
        }
    }
}