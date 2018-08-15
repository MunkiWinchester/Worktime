﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PublicHoliday;
using Worktime.DataObjetcs;
using Worktime.Extension;
using Worktime.Properties;

namespace Worktime.Business
{
    public static class EmployeeManager
    {
        private static DirectoryInfo JsonSaveLocation
        {
            get
            {

                var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                DirectoryInfo dirInfo = null;
                if (location != null)
                {
#if DEBUG
                    dirInfo = new DirectoryInfo(Path.Combine(location, "Data", "Test"));
#else
                    dirInfo = new DirectoryInfo(Path.Combine(location, "Data"));
#endif
                }
                return dirInfo;
            }
        }

        public static List<Employee> GetEmployeeValuesFromJson()
        {
            var conDic = new ConcurrentDictionary<string, Employee>();
            try
            {
                var dirInfo = JsonSaveLocation;
                if (dirInfo != null)
                {
                    var fileInfos = dirInfo.GetFiles("*.json", SearchOption.TopDirectoryOnly);
                    Parallel.ForEach(fileInfos,
                        fileInfo =>
                        {
                            var name = Path.GetFileNameWithoutExtension(fileInfo.Name);
                            if (!conDic.ContainsKey(name))
                            {
                                var employee = JsonConvert.DeserializeObject<Employee>(File.ReadAllText(fileInfo.FullName));
                                conDic.TryAdd(name, employee);
                            }
                        });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return conDic.Values.ToList();
        }

        private static Employee GetCurrentEmployeeValueFromJson()
        {
            var dirInfo = JsonSaveLocation;
            if (dirInfo != null)
            {
                var fileInfos = dirInfo.GetFiles(DateTime.Now.Iso8601WeekOfYear().FileName);
                if (fileInfos.Any())
                    return JsonConvert.DeserializeObject<Employee>(File.ReadAllText(fileInfos[0].FullName));
            }
            return new Employee();
        }

        /// <summary>
        /// Returns the values for the given personal number
        /// </summary>
        /// <returns></returns>
        public static Employee LoadEmployeeValues()
        {

            var employee = GetCurrentEmployeeValueFromJson();
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

        public static TimeSpan CalculateTotalOvertime(Employee employee)
        {
            var overtime = new TimeSpan(0);
            var employeeValues = GetEmployeeValuesFromJson();
            foreach (var employeeValue in employeeValues.Where(ev => !ev.IsoWeek.Equals(employee.IsoWeek)))
            {
                overtime += employeeValue.WeekWorkTimeReal - employeeValue.WeekWorkTimeRegular;
            }
            foreach (var time in employee.Times)
            {
                overtime += time.SpanCorrected - employee.WorkTimeRegular;
            }
            return overtime;
        }

        public static void SaveEmployeeValues(Employee employee)
        {
            var json = JsonConvert.SerializeObject(employee,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat
                });
            var dirInfo = JsonSaveLocation;
            if (dirInfo != null)
                using (var sw = new StreamWriter(Path.Combine(dirInfo.FullName, employee.IsoWeek.FileName)))
                {
                    sw.Write(json);
                }
        }

        public static int CalculateHolidays()
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

        public static void AddStamp(Employee employee)
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