using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roughcut.DataMartServices.Core.Enums;
using Roughcut.DataMartServices.Infrastructure.DbModels;

//using Dapper;

namespace Roughcut.DataMartServices.Infrastructure.Helpers
{
    //public static class DWDateTools
    //{

    public static class DateTimeHelper
    {
        static DateTimeHelper()
        {
            // tbd
        }

        // 200507010000 / 200507010030
        public static long CreateDateTimeKey(DateTime dateTimeToUse)
        {

            //
            long dateTimeTemp = -1;
            string dateTimeString = "";

            // - year
            dateTimeString = GetCalendarFourDigitYearString(dateTimeToUse);

            // - two-digit month
            dateTimeString += GetCalendarTwoDigitMonthString(dateTimeToUse);

            // - two-digit day
            dateTimeString += GetCalendarTwoDigitDayString(dateTimeToUse).ToString();

            // - two-digit hour
            dateTimeString += GetCalendarTwoDigitHourString(dateTimeToUse).ToString();

            // - two-digit minutes (grain: 30 mins)
            dateTimeString += GetCalendarTwoDigitMinuteString(dateTimeToUse).ToString();



            // cast from string-to-long (bigint)
            dateTimeTemp = Convert.ToInt64(dateTimeString);

            // return
            return dateTimeTemp;

        }

        //
        private static string GetCalendarTwoDigitMinuteString(DateTime dateTimeToUse)
        {
            return dateTimeToUse.Minute.ToString("d2");
        }

        //
        private static string GetCalendarTwoDigitHourString(DateTime dateTimeToUse)
        {
            return dateTimeToUse.Hour.ToString("d2");
        }

        //
        private static string GetCalendarTwoDigitDayString(DateTime dateTimeToUse)
        {
            return dateTimeToUse.Day.ToString("d2");
        }

        //
        private static string GetCalendarFourDigitYearString(DateTime dateTimeToUse)
        {
            return dateTimeToUse.ToString("yyyy");
        }

        //
        private static string GetCalendarTwoDigitMonthString(DateTime dateTimeToUse)
        {
            return dateTimeToUse.Month.ToString("d2");
        }



        // EnglishDayNameOfWeek - Monday-Friday, etc.
        public static string GetEnglishDayOfWeekLong(DateTime dateTimeToUse)
        {
            return dateTimeToUse.ToString("dddd");
        }

        // EnglishDayNameOfWeek - Mon-Fri, etc.
        public static string GetEnglishDayOfWeekShort(DateTime dateTimeToUse)
        {
            return dateTimeToUse.ToString("dd");
        }

        //
        public static short GetCalendarYear(DateTime dateTimeToUse)
        {
            return short.Parse(dateTimeToUse.ToString("yyyy"));
        }


        // http://stackoverflow.com/questions/21266857/how-to-get-current-quarter-from-current-date-using-c-sharp
        public static short GetCalendarQuarter(DateTime dateTimeToUse)
        {
            // two way, opted for the simpler way - Houston - 02.21.17
            // test-stubs, using linqpad (linqpad.net) 
            //Math.Ceiling(testDT.Month / 3m).Dump();
            //((testDT.Month + 2) / 3).Dump();
            return (short) ((dateTimeToUse.Month + 2) / 3);
        }


        //
        public static short GetCalendarMonth(DateTime dateTimeToUse)
        {
            return short.Parse(dateTimeToUse.ToString("MM"));
        }

        //
        public static short GetCalendarTwoDigitDay(DateTime dateTimeToUse)
        {
            return short.Parse(dateTimeToUse.Day.ToString("d2"));
        }


        public static List<DimDateTime> GetDimDateTimesDataAll(string dbConnString)
        {

            //
            List<DimDateTime> dimDateTimes = new List<DimDateTime>(); // connection.Query<DimDateTime>(sqlToUse).ToList();

            //// 
            //using (SqlConnection connection = DbTools.GetOpenConnection(dbConnString))
            //{

            //    // get-sql
            //    string sqlToUse =
            //        String.Format(@"SELECT [DateTimeKey]
            //                          ,[DateTimeCalendarKey]
            //                          ,[CalendarYear]
            //                          ,[CalendarMonth]
            //                          ,[CreateDateTime]
            //                          ,[LastChangeDateTime]
            //                      FROM [dbo].[DimDateTime];");

            //}

            // return
            return dimDateTimes;

        }

        public static DateTime SetDateTimeKeyByGrain(DateTimeGrainTypes grain, DateTime currentDimItem)
        {
//
            switch (grain)
            {
                case DateTimeGrainTypes.ThirtyMinutes:
                    currentDimItem = currentDimItem.AddMinutes(30);
                    break;
                case DateTimeGrainTypes.Hourly:
                    currentDimItem = currentDimItem.AddHours(1);
                    break;
                case DateTimeGrainTypes.Daily:
                    currentDimItem = currentDimItem.AddDays(1);
                    break;
                case DateTimeGrainTypes.Weekly:
                    currentDimItem = currentDimItem.AddDays(7);
                    break;
                case DateTimeGrainTypes.Monthly:
                    currentDimItem = currentDimItem.AddMonths(1);
                    break;
                case DateTimeGrainTypes.Quarterly:
                    currentDimItem = currentDimItem.AddMonths(3);
                    break;

                case DateTimeGrainTypes.Yearly:
                    currentDimItem = currentDimItem.AddYears(1);
                    break;

                default:
                    currentDimItem = currentDimItem.AddHours(1);
                    break;
            }
            return currentDimItem;
        }
    }
}
