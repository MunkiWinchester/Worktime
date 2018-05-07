using System;
using Newtonsoft.Json;
using WpfUtility;

namespace Worktime.DataObjetcs
{
    public class TimeFrame : ObservableObject
    {
        private TimeSpan _begin;
        private TimeSpan? _end;

        public TimeSpan Begin
        {
            get => _begin;
            set
            {
                _begin = new TimeSpan(value.Hours, value.Minutes, value.Seconds);
                OnPropertyChanged();
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
            }
        }

        [JsonIgnore]
        public TimeSpan Span =>
            End != null
                ? (TimeSpan) End - Begin
                : new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second) - Begin;

        public override string ToString()
        {
            return $"{Begin:dd.MM. HH:mm} --- {End:dd.MM. HH:mm}";
        }
    }
}
