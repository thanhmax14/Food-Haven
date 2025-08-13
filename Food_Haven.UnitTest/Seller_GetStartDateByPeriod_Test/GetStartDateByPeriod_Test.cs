using System;
using NUnit.Framework;

namespace Food_Haven.UnitTest.Seller_GetStartDateByPeriod_Test
{
    public class GetStartDateByPeriod_Test
    {
        // Forcing "now" in tests for determinism
        private readonly DateTime _fixedNow = new DateTime(2025, 8, 13, 8, 56, 43);

        private DateTime GetStartDateByPeriod(string period, DateTime? now = null)
        {
            DateTime actualNow = now ?? DateTime.Now;
            return period.ToLower() switch
            {
                "today" => actualNow.Date,
                "yesterday" => actualNow.Date.AddDays(-1),
                "last7days" => actualNow.Date.AddDays(-7),
                "last30days" => actualNow.Date.AddDays(-30),
                "thismonth" => new DateTime(actualNow.Year, actualNow.Month, 1),
                "lastmonth" => new DateTime(actualNow.Year, actualNow.Month, 1).AddMonths(-1),
                "alltime" => DateTime.MinValue,
                _ => actualNow.Date
            };
        }

        [Test]
        public void Today_ReturnsStartOfToday()
        {
            var result = GetStartDateByPeriod("today", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 8, 13), result);
        }

        [Test]
        public void Yesterday_ReturnsStartOfYesterday()
        {
            var result = GetStartDateByPeriod("yesterday", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 8, 12), result);
        }

        [Test]
        public void Last7Days_ReturnsStartOfLast7Days()
        {
            var result = GetStartDateByPeriod("last7days", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 8, 6), result);
        }

        [Test]
        public void Last30Days_ReturnsStartOfLast30Days()
        {
            var result = GetStartDateByPeriod("last30days", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 7, 14), result);
        }

        [Test]
        public void ThisMonth_ReturnsFirstDayOfMonth()
        {
            var result = GetStartDateByPeriod("thismonth", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 8, 1), result);
        }

        [Test]
        public void LastMonth_ReturnsFirstDayOfLastMonth()
        {
            var result = GetStartDateByPeriod("lastmonth", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 7, 1), result);
        }

        [Test]
        public void AllTime_ReturnsDateTimeMinValue()
        {
            var result = GetStartDateByPeriod("alltime", _fixedNow);
            Assert.AreEqual(DateTime.MinValue, result);
        }

        [Test]
        public void UnknownPeriod_ReturnsStartOfToday()
        {
            var result = GetStartDateByPeriod("somethingelse", _fixedNow);
            Assert.AreEqual(new DateTime(2025, 8, 13), result);
        }
    }
}