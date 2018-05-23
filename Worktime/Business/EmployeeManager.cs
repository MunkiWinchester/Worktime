using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PublicHoliday;
using Worktime.DataObjetcs;
using Worktime.Properties;

namespace Worktime.Business
{
    public class EmployeeManager
    {
        public static string GetEmployeeJsonPath()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (location != null)
#if DEBUG
                return Path.Combine(location,
                    "TestEmployee.json");
#else
                return Path.Combine(location,
                    "Employee.json");
#endif
            return string.Empty;
        }

        /// <summary>
        /// Returns the values for the given personal number
        /// </summary>
        /// <returns></returns>
        public Employee LoadEmployeeValues()
        {
            var employee = new Employee();
            try
            {
                employee = JsonConvert.DeserializeObject<Employee>(File.ReadAllText(GetEmployeeJsonPath()));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var removeItems = employee.Times.Where(t => t.Date.Iso8601WeekOfYear() != DateTime.Now.Iso8601WeekOfYear())
                .ToList();
            foreach (var removeItem in removeItems)
                employee.Times.Remove(removeItem);

            employee.Times = new ObservableCollection<Times>(employee.Times.OrderBy(t => t.Date));
            foreach (var employeeTime in employee.Times)
                employeeTime.TimeFrames =
                    new ObservableCollection<TimeFrame>(employeeTime.TimeFrames.OrderBy(tf => tf.Begin));

            if (employee.Times.All(x => x.Date.DayOfYear != DateTime.Now.DayOfYear))
                employee.Times.Add(new Times
                {
                    Date = DateTime.Now.ToTimelessDateTime(),
                    TimeFrames = new ObservableCollection<TimeFrame>
                    {
                        new TimeFrame {Begin = DateTime.Now.ToDatelessTimeSpan(), IsCurrent = true}
                    }
                });

            employee.WeekWorkTimeRegular =
                TimeSpan.FromMilliseconds((5 - CalculateHolidays()) * employee.WorkTimeRegular.TotalMilliseconds);

            employee.BreakTimeRegular = new TimeSpan(0,
                Convert.ToInt32(Math.Floor(employee.WorkTimeRegular.TotalHours / 3) * 15), 0);

            SaveEmployeeValues(employee);

            return employee;
        }

        public void SaveEmployeeValues(Employee employee)
        {
            var json = JsonConvert.SerializeObject(employee,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                });
            using (var sw = new StreamWriter(GetEmployeeJsonPath()))
            {
                sw.Write(json);
            }
        }

        public int CalculateHolidays()
        {
            var calendar = new GermanPublicHoliday
            {
                State = ((GermanPublicHoliday.States[]) Enum.GetValues(typeof(GermanPublicHoliday.States)))
                    .FirstOrDefault(x => x.ToString().Equals(Settings.Default.SelectedState))
            };
            var publicHolidays = calendar.PublicHolidays(DateTime.Now.Year).ToList();

            var days = publicHolidays.Where(p =>
                    p >= DateTime.Now.StartOfWeek(DayOfWeek.Monday) && p <= DateTime.Now.EndOfWeek(DayOfWeek.Monday))
                .ToList();
            return days.Count;
        }

        public void AddStamp(Employee employee)
        {
            if (employee.Times.All(t => t.Date.DayOfYear != DateTime.Now.DayOfYear))
            {
                employee.Times.Add(new Times
                {
                    Date = DateTime.Now.ToTimelessDateTime(),
                    TimeFrames = new ObservableCollection<TimeFrame>
                    {
                        new TimeFrame {Begin = DateTime.Now.ToDatelessTimeSpan(), IsCurrent = true}
                    }
                });
            }
            else
            {
                var time = employee.Times.FirstOrDefault(t => t.Date.DayOfYear == DateTime.Now.DayOfYear);
                if (time != null)
                    if (time.TimeFrames.All(tf => tf.End != null))
                    {
                        foreach (var timeFrame in time.TimeFrames.Where(tf => tf.IsCurrent))
                            timeFrame.IsCurrent = false;

                        time.TimeFrames.Add(new TimeFrame
                        {
                            Begin = DateTime.Now.ToDatelessTimeSpan(),
                            IsCurrent = true
                        });
                    }
                    else
                    {
                        var timeframe = time.TimeFrames.FirstOrDefault(tf => tf.End == null);
                        if (timeframe != null)
                        {
                            timeframe.End = DateTime.Now.ToDatelessTimeSpan();
                            timeframe.IsCurrent = false;
                        }
                    }
            }

            SaveEmployeeValues(employee);
        }
    }
}