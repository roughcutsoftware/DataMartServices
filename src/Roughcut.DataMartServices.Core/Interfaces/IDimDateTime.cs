using System;

namespace Roughcut.DataMartServices.Core.Interfaces
{
    public interface IDimDateTime
    {
        long DimDateTimeKeyId { get; set; }
        DateTime DateTimeCalendarKey { get; set; }
        short DayNumberOfWeek { get; set; }
        string EnglishDayNameOfWeek { get; set; }
        DateTime CreateDateTime { get; set; }
        DateTime LastChangeDateTime { get; set; }
    }
}