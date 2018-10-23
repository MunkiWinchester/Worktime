using System;
using Newtonsoft.Json;
using Worktime.Extension;
using WpfUtility.Services;

namespace Worktime.DataObjetcs
{
    public class TimeFrame : ObservableObject
    {
        public event EventHandler<OnChangeEventArgs> OnChange;
        private TimeSpan _begin;
        private TimeSpan? _end;
        private bool _isCurrent;
        private readonly bool _isInitial = true;

        public TimeFrame()
        {
            Begin = DateTime.Now.ToDatelessTimeSpan();
            _isInitial = false;
        }

        public bool IsCurrent
        {
            get => _isCurrent;
            set
            {
                if (SetField(ref _isCurrent, value) && !_isInitial)
                    OnChange?.Invoke(this, new OnChangeEventArgs(nameof(IsCurrent)));
            }
        }

        public TimeSpan Begin
        {
            get => _begin;
            set
            {
                if (SetField(ref _begin, new TimeSpan(value.Hours, value.Minutes, value.Seconds)))
                {
                    if(!_isInitial)
                        OnChange?.Invoke(this, new OnChangeEventArgs(nameof(Begin)));
                    OnPropertyChanged(nameof(Span));
                }
            }
        }

        public TimeSpan? End
        {
            get => _end;
            set
            {
                var end = new TimeSpan?();
                if (value is TimeSpan notNullValue && notNullValue != TimeSpan.Zero)
                    end = new TimeSpan(notNullValue.Hours, notNullValue.Minutes, notNullValue.Seconds);
                else
                    end = null;
                if (SetField(ref _end, end))
                {
                    if (!_isInitial)
                        OnChange?.Invoke(this, new OnChangeEventArgs(nameof(End)));
                    OnPropertyChanged(nameof(Span));
                }
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
            return $"{Begin:hh\\:mm} --- {End:hh\\:mm}";
        }
    }
}