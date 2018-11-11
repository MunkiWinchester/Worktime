using System;
using Newtonsoft.Json;
using Worktime.Extension;

namespace Worktime.DataObjetcs
{
    public class IsoWeek : IEquatable<IsoWeek>, IComparable<IsoWeek>
    {
        public int Number { get; set; }
        public int Year { get; set; }
        [JsonIgnore]
        public string FileName => $"{Number}_{Year}.json";

        public override string ToString()
        {
            return $"{Number} / {Year}";
        }

        public IsoWeek()
        {
            var iso =  DateTime.Now.Iso8601WeekOfYear();
            Number = iso.Number;
            Year = iso.Year;
        }

        public IsoWeek(int number, int year)
        {
            Number = number;
            Year = year;
        }

        public bool IsCurrent
        {
            get
            {
                var currentIsoWeek = DateTime.UtcNow.Iso8601WeekOfYear();
                if (Equals(currentIsoWeek))
                    return true;
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is IsoWeek otherIsoWekk)
                return Number == otherIsoWekk.Number &&
                       Year == otherIsoWekk.Year;
            return base.Equals(obj);
        }

        public bool Equals(IsoWeek other)
        {
            return other != null &&
                   Number == other.Number &&
                   Year == other.Year;
        }

        public override int GetHashCode()
        {
            var hashCode = 545733884;
            hashCode = hashCode * -1521134295 + Number.GetHashCode();
            hashCode = hashCode * -1521134295 + Year.GetHashCode();
            return hashCode;
        }

        public int CompareTo(IsoWeek other)
        {
            if (Year > other.Year ||
                (Year == other.Year && Number > other.Number))
                return 1;
            if (Year == other.Year && Number == other.Number)
                return 0;
            return -1;
        }
    }
}
