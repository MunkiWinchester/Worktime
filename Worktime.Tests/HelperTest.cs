using System;
using System.Collections.Generic;
using Xunit;

namespace XUnitTestProject1
{
    public class HelperTest
    {
        public static IEnumerable<object[]> CalculateRegularBreakTestData
        {
            get
            {
                return new[]
                {
                    new object[] { new TimeSpan(7, 23, 0), new TimeSpan(8, 0, 0), new TimeSpan(0, 30, 0) },
                    new object[] { new TimeSpan(9, 23, 0), new TimeSpan(8, 0, 0), new TimeSpan(0, 45, 0) }
                };
            }
        }

        public static IEnumerable<object[]> CalculatePercentageTestData
        {
            get
            {
                return new[] 
                {
                    new object[] { new TimeSpan(4, 0, 0), new TimeSpan(8, 0, 0), 50d },
                    new object[] { new TimeSpan(6, 0, 0), new TimeSpan(8, 0, 0), 75d },
                    new object[] { new TimeSpan(8, 0, 0), new TimeSpan(8, 0, 0), 100d }
                };
            }
        }

        [Theory, MemberData(nameof(CalculateRegularBreakTestData))]
        public void CalculateRegularBreakTest(TimeSpan workTimeReal, TimeSpan workTimeRegular, TimeSpan expected)
        {
            var actual = Worktime.Business.Helper.CalculateRegularBreak(workTimeReal, workTimeRegular);
            Assert.Equal(expected, actual);
        }

        [Theory, MemberData(nameof(CalculatePercentageTestData))]
        public void CalculatePercentageTest(TimeSpan actualTime, TimeSpan regularTime, double expected)
        {
            var actual = Worktime.Business.Helper.CalculatePercentage(actualTime, regularTime);
            Assert.Equal(expected, actual);
        }
    }
}
