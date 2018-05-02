using System;
using Newtonsoft.Json;
using WpfUtility;

namespace Worktime.DataObjetcs
{
    public class TimeFrame : ObservableObject
    {
        private DateTime _begin;
        private DateTime? _end;

        public DateTime Begin
        {
            get => _begin;
            set
            {
                _begin = value;
                OnPropertyChanged();
            }
        }

        public DateTime? End
        {
            get => _end;
            set
            {
                _end = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public TimeSpan Span => End != null ? (DateTime) End - Begin : DateTime.Now - Begin;

        public override string ToString()
        {
            return $"{Begin:dd.MM. HH:mm} --- {End:dd.MM. HH:mm}";
        }
    }
}
