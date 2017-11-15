using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HisRoyalRedness.com;
using FluentAssertions;

namespace HisRoyalRedness.com.Test
{
    [TestClass]
    public class DateTimeInstanceModel_Test
    {
        //[TestMethod]
        //public void TestConstruction_default_time_and_zone()
        //{
        //    var dt1 = new DateTimeInstanceModel();
        //    dt1.TimeZone.Should().Be(TimeZoneInfo.Utc, "default time zone should be UTC.");
        //    dt1.CommonTime.Should().BeCloseTo(DateTime.Now.ToUniversalTime(), because: "default time in Now");
        //    dt1.CommonTime.ToUniversalTime().Should().Be(dt1.DisplayTime.ToUniversalTime(), "display and common time should both be UTC");
        //}

        //[TestMethod]
        //public void TestConstruction_specific_time_default_zone()
        //{
        //    var dt1 = new DateTimeInstanceModel(DateTime.Parse(_testTime).ToUniversalTime());
        //    dt1.TimeZone.Should().Be(TimeZoneInfo.Utc, "default time zone should be UTC.");
        //    dt1.CommonTime.Should().Be(DateTime.Parse(_testTime).ToUniversalTime());
        //    dt1.DisplayTime.Should().Be(DateTime.Parse(_testTime).ToUniversalTime());
        //    dt1.CommonTime.ToUniversalTime().Should().Be(dt1.DisplayTime.ToUniversalTime(), "display and common time should both be UTC");
        //}

        //[TestMethod]
        //public void TestConstruction_specific_time_zone()
        //{
        //    var dt1 = new DateTimeInstanceModel(TimeZoneInfo.Local, DateTime.Parse(_testTime));
        //    dt1.TimeZone.Should().Be(TimeZoneInfo.Utc, "default time zone should be UTC.");
        //    dt1.CommonTime.Should().Be(DateTime.Parse(_testTime));
        //    dt1.DisplayTime.Should().Be(DateTime.Parse(_testTime).ToUniversalTime());
        //    dt1.CommonTime.ToUniversalTime().Should().Be(dt1.DisplayTime.ToUniversalTime(), "display and common time should both be UTC");
        //}

        //[TestMethod]
        //public void TestConversion()
        //{
        //    var dt1 = new DateTimeInstanceModel();
        //    dt1.CommonTime = _common;
        //    dt1.DisplayTime.Should().Be(DateTime.Parse("2017-11-08 02:08:45 +13"));
        //}

        //readonly string _testTime = "2017-11-07 13:08:45";
        //readonly DateTime _common = new DateTime(2017, 11, 7, 13, 8, 45, DateTimeKind.Utc);
    }
}
