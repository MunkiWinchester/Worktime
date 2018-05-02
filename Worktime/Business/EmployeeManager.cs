﻿using System;
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
                return Path.Combine(location, "Resources",
                    "Employee.json");
            return string.Empty;
        }

        /// <summary>
        ///     Returns the values for the given personal number
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

            if (employee.Times.All(t => t.Date.Iso8601WeekOfYear() != DateTime.Now.Iso8601WeekOfYear()))
                employee.Times.Clear();

            //if (employee.TimeFrames.All(t => t.Begin.DayOfYear != DateTime.Now.DayOfYear))
            //    employee.TimeFrames.Add(new TimeFrame {Begin = DateTime.Now});

            employee.WeekWorkTimeRegular =
                TimeSpan.FromMilliseconds((5 - CalculateHolidays()) * employee.WorkTimeRegular.TotalMilliseconds);

            employee.BreakTimeRegular = new TimeSpan(0,
                Convert.ToInt32(Math.Floor(employee.WorkTimeRegular.TotalHours / 3) * 15), 0);

            //foreach (var timeFrame in employee.TimeFrames.Where(t => !t.Finished))
            //{
            //    timeFrame.End = DateTime.Now;
            //}

            return employee;
        }

        public void SaveEmployeeValues(Employee employee)
        {
            var json = JsonConvert.SerializeObject(employee,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Formatting = Formatting.Indented
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
    }
}
