using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Roughcut.DataMartServices.Core.Enums;
using Roughcut.DataMartServices.Infrastructure.DbContexts;
using Roughcut.DataMartServices.Infrastructure.DbModels;
using Roughcut.DataMartServices.Infrastructure.Extensions;
using Roughcut.DataMartServices.Infrastructure.Helpers;

namespace Roughcut.DataMartServices.Infrastructure.Services
{
    public class DataMartService
    {
        //public static void CreateDimDateTimeDimension(string dbConnString)
        //{

        //    ////
        //    ////var beginDateTime = DateTime.Parse("01/01/2005 12:00 AM");
        //    ////var dimDateTimeEnd = DateTime.Parse("12/31/2025 11:30 PM");
        //    //var beginDateTime = DateTime.Parse("01/01/2008 12:00 AM");
        //    //var dimDateTimeEnd = DateTime.Parse("12/31/2028 11:30 PM");

        //    ////
        //    //CreateDateTimeDimension(dbConnString, DateTimeGrainTypes.ThirtyMinutes);

        //}

        public static string CreateDateTimeDimension(string dbConnString, DateTime beginDateTimeYear, int numberOfYears, DateTimeGrainTypes grain)
        {

            //
            try
            {


                // setup being/end datetime ranges - examples:
                //var beginDateTime = DateTime.Parse("01/01/2005 12:00 AM");
                //var dimDateTimeEnd = DateTime.Parse("12/31/2025 11:30 PM");

                string beginYearString = beginDateTimeYear.Year.ToString();

                int endYear = beginDateTimeYear.Year + numberOfYears;
                string endYearString = endYear.ToString();

                //
                string beginDateTimeString = @"01/01/" + beginYearString.Trim() + " 12:00 AM";
                string endDateTimeString = @"12/31/" + endYearString.Trim() + " 11:30 PM";

                //string beginDateTimeString = @"01/01/1995 12:00 AM";
                //string endDateTimeString = @"12/31/1999 11:30 PM";

                //DateTime beginDateTime = DateTime.Parse(beginDateTimeString);
                //DateTime endDateTime = DateTime.Parse(endDateTimeString);

                DateTime beginDateTime = Convert.ToDateTime(beginDateTimeString);
                DateTime endDateTime = Convert.ToDateTime(endDateTimeString);

                //DateTime beginDateTime = new DateTime(1995,1,1,0,0,0);
                //DateTime endDateTime = new DateTime(1999,12,31,23,30,0);


                //
                int batchSize = 10000;
                int currentBatchSizeCounter = 1;
                int speedTestCounter = 1;
                DimDateTime currentDimDateTime = null;
                List<DimDateTime> dimDateTimeBatchList = new List<DimDateTime>();

                // start at the beginning and start-looping - Houston - 02.21.17
                DateTime tempDateTime = beginDateTime;

                //
                //using (GenericDWDbContexts db = new GenericDWDbContexts())
                using (DataMartServicesDbContext db = new DataMartServicesDbContext(dbConnString))
                {

                    //
                    do
                    {
                        // always initialize new instance 
                        currentDimDateTime = new DimDateTime();

                        // set datetime-dim attribs - Houston - 02.21.17
                        currentDimDateTime.DimDateTimeKeyId = DateTimeHelper.CreateDateTimeKey(tempDateTime);
                        currentDimDateTime.DateTimeCalendarKey = tempDateTime;

                        currentDimDateTime.DayNumberOfWeek = DateTimeHelper.GetCalendarTwoDigitDay(tempDateTime);
                        currentDimDateTime.EnglishDayNameOfWeek = DateTimeHelper.GetEnglishDayOfWeekLong(tempDateTime);

                        //currentDimDateTime.DayNumberOfWeek = DateTimeHelper.GetCalendarTwoDigitDay(tempDateTime);
                        //currentDimDateTime.DayNumberOfWeek = DateTimeHelper.GetCalendarTwoDigitDay(tempDateTime);


                        // add to list 
                        dimDateTimeBatchList.Add(currentDimDateTime);

                        // if list >= batchSize, save-to-db
                        if (dimDateTimeBatchList.Count >= batchSize)
                        {
                            string debug = "breakpoint-placeholder";

                            //// commit to disk (db)
                            //if (speedTestCounter == 1)
                            //{
                            //    Stopwatch watch = Stopwatch.StartNew();

                            //    //
                            //    db.DimDateTime.AddRange(dimDateTimeBatchList);
                            //    db.SaveChanges();

                            //    //
                            //    watch.Stop();
                            //    Console.WriteLine("EF-SaveChanges-10000-rows: " + "\t" + StopwatchElapsedFormatted(watch));

                            //}
                            //else if (speedTestCounter == 2)
                            //{
                            //    // 
                            //    Stopwatch watch = Stopwatch.StartNew();
                            //    using (SqlConnection dbConn = DbTools.GetOpenConnection(dbConnString))
                            //    {
                            //        //
                            //        int result = dbConn.Insert(dimDateTimeBatchList);//.Query<List<DimDateTime>>(sqlToUse).ToList();
                            //    }
                            //    watch.Stop();
                            //    Console.WriteLine("Dapper-wContrib-Insert-10000-rows: " + "\t" + StopwatchElapsedFormatted(watch));

                            //}
                            //else if(speedTestCounter == 3)
                            //{
                            //
                            CommitDimDateTimeBatchListToDb(dbConnString, dimDateTimeBatchList, currentBatchSizeCounter);


                            //}
                            //else
                            //{
                            //    return;

                            //}

                            //
                            speedTestCounter += 1;
                            //currentBatchSizeCounter = 1;
                            dimDateTimeBatchList = new List<DimDateTime>();

                        }

                        // increase current-batch-size counter
                        currentBatchSizeCounter += 1;

                        // increase temp-datetime for next iteration, by 'grain'
                        tempDateTime = DateTimeHelper.SetDateTimeKeyByGrain(grain, tempDateTime);

                        //
                        // only for very-last batch, which may be less than batchSize
                        if (currentDimDateTime.DateTimeCalendarKey >= DateTime.Parse("2025-07-14 23:30:00.000"))
                        //if (dimDateTimeEnd == currentDimDateTime.DateTimeCalendarKey && dimDateTimeBatchList.Count > 0 &&
                        //    dimDateTimeBatchList.Count < batchSize)
                        {
                            string debug = "placeholder";
                        }

                        // only for very-last batch, which may be less than batchSize
                        if (endDateTime == currentDimDateTime.DateTimeCalendarKey
                            && dimDateTimeBatchList.Count > 0
                            && dimDateTimeBatchList.Count < batchSize)
                        {
                            string debug = "placeholder";
                            //
                            CommitDimDateTimeBatchListToDb(dbConnString, dimDateTimeBatchList, currentBatchSizeCounter);
                        }

                        // 
                    }
                    //while (endDateTime >= currentDimDateTime.DateTimeCalendarKey);
                    while (currentDimDateTime.DateTimeCalendarKey <= endDateTime);



                    //
                }  //

            }
            catch (Exception exc)
            {
                //Console.WriteLine(exc);
                //throw;

                return "failed";

            }


            // DateTimeHelper

            //// 2. - create 'target' datatable
            //DataTable dataTableDimDateTime = DateTimeDimTools.CreateDimDateTimeDataTable(DateTimeGrainTypes.Weekly
            //    , beginDateTime);

            //// 3. - bulkCopy 'target' datatable to destination-db
            //SqlBulkCopyHelper.CopyDataTableToSourceTable(dbConnString, dataTableDimDateTime, "DimDateTime");

            //
            return "success";

        }

        private static void CommitDimDateTimeBatchListToDb(string dbConnString, List<DimDateTime> dimDateTimeBatchList,
            int currentBatchSizeCounter)
        {
            Stopwatch watch = Stopwatch.StartNew();

            // http://stackoverflow.com/questions/564366/convert-generic-list-enumerable-to-datatable
            DataTable dataTableToBulkCopy = dimDateTimeBatchList.ToDataTable();

            // 3. - bulkCopy 'target' datatable to destination-db
            SqlBulkCopyHelper.CopyDataTableToSourceTable(dbConnString, dataTableToBulkCopy, "DimDateTime");
            watch.Stop();

            //// must-refactor - good code - Houston - 09.20.19
            //Console.WriteLine("List-To-DataTable-BulkCopy-{0}-rows: " + "\t" + StopwatchElapsedFormatted(watch),
            //    currentBatchSizeCounter.ToString());
        }

        //
        public static List<string> GetDimDateTimeTableCreateSqlScriptBlocks()
        {

            //
            List<string> sqlScriptBlocks = new List<string>();


//            //
//            sqlScriptBlocks.Add(@"
//                            CREATE TABLE [dbo].[DimDateTime](
//	                            [DimDateTimeKeyId] [bigint] NOT NULL,
//	                            [DateTimeCalendarKey] [datetime] NOT NULL,
//	                            [DayNumberOfWeek] [smallint] NOT NULL,
//	                            [EnglishDayNameOfWeek] [varchar](10) NOT NULL,
//	                            [CreateDateTime] [datetime] NOT NULL,
//	                            [LastChangeDateTime] [datetime] NOT NULL,
//                             CONSTRAINT [PK_DimDateTime] PRIMARY KEY CLUSTERED 
//                            (
//	                            [DimDateTimeKeyId] ASC
//                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
//                            ) ON [PRIMARY]

//                            GO
//                            /****** Object:  Index [ix_calendar_key_incl_dimkeyid_dayofweek]    Script Date: 9/20/2019 11:36:52 PM ******/
//                            CREATE NONCLUSTERED INDEX [ix_calendar_key_incl_dimkeyid_dayofweek] ON [dbo].[DimDateTime]
//                            (
//	                            [DateTimeCalendarKey] ASC
//                            )
//                            INCLUDE ( 	[DimDateTimeKeyId],
//	                            [EnglishDayNameOfWeek]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
//                            GO
//                            /****** Object:  Index [ix_datetime_calendar_key_incl_dimkeyid]    Script Date: 9/20/2019 11:36:52 PM ******/
//                            CREATE UNIQUE NONCLUSTERED INDEX [ix_datetime_calendar_key_incl_dimkeyid] ON [dbo].[DimDateTime]
//                            (
//	                            [DateTimeCalendarKey] ASC
//                            )
//                            INCLUDE ( 	[DimDateTimeKeyId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
//                            GO
//");

            //
            sqlScriptBlocks.Add(@"
                            CREATE TABLE [dbo].[DimDateTime](
	                            [DimDateTimeKeyId] [bigint] NOT NULL,
	                            [DateTimeCalendarKey] [datetime] NOT NULL,
	                            [DayNumberOfWeek] [smallint] NOT NULL,
	                            [EnglishDayNameOfWeek] [varchar](10) NOT NULL,
	                            [CreateDateTime] [datetime] NOT NULL,
	                            [LastChangeDateTime] [datetime] NOT NULL,
                             CONSTRAINT [PK_DimDateTime] PRIMARY KEY CLUSTERED 
                            (
	                            [DimDateTimeKeyId] ASC
                            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                            ) ON [PRIMARY]
");


            //
            sqlScriptBlocks.Add(@"
                            /****** Object:  Index [ix_calendar_key_incl_dimkeyid_dayofweek]    Script Date: 9/20/2019 11:36:52 PM ******/
                            CREATE NONCLUSTERED INDEX [ix_calendar_key_incl_dimkeyid_dayofweek] ON [dbo].[DimDateTime]
                            (
	                            [DateTimeCalendarKey] ASC
                            )
                            INCLUDE ( 	[DimDateTimeKeyId],
	                            [EnglishDayNameOfWeek]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
");


            //
            sqlScriptBlocks.Add(@"
                            /****** Object:  Index [ix_datetime_calendar_key_incl_dimkeyid]    Script Date: 9/20/2019 11:36:52 PM ******/
                            CREATE UNIQUE NONCLUSTERED INDEX [ix_datetime_calendar_key_incl_dimkeyid] ON [dbo].[DimDateTime]
                            (
	                            [DateTimeCalendarKey] ASC
                            )
                            INCLUDE ( 	[DimDateTimeKeyId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
");




            //
            return sqlScriptBlocks;
        }

        //
        public static List<string> GetSqlScalarFunction_GetDimDateTimeKeyIdForFactValueCreateSqlScriptBlocks()
        {

            //
            List<string> sqlScriptBlocks = new List<string>();

            //
            sqlScriptBlocks.Add(@"
                                /****** Object:  UserDefinedFunction [dbo].[svf_GetDimDateTimeKeyIdForFactValue]    Script Date: 9/21/2019 12:12:09 PM ******/
                                SET ANSI_NULLS ON
                                ;
                                SET QUOTED_IDENTIFIER ON
                                ;
                                ");

            //
            sqlScriptBlocks.Add(@"
                                -- created by Houston - 01.30.18
                                CREATE FUNCTION [dbo].[svf_GetDimDateTimeKeyIdForFactValue] (
	                                @paramDateTime DATETIME
                                )

                                RETURNS bigint
                                AS
                                BEGIN
                                    DECLARE @dateTimeKeyId bigint


                                --@paramDateTime
                                SELECT TOP 1
	                                @dateTimeKeyId = [DimDateTimeKeyId]

                                FROM [dbo].[DimDateTime]
                                WHERE [DateTimeCalendarKey] >= @paramDateTime

                                RETURN @dateTimeKeyId

                                END
                                ");




            //
            return sqlScriptBlocks;
        }


        public static long PurgeDimDateTimeTable(string dbConnString)
        {
            long result = 0;
            using (SqlConnection dbConn = new SqlConnection(dbConnString))
            {
                SqlCommand sqlCmd = new SqlCommand();

                // open connection 
                dbConn.Open();

                sqlCmd.Connection = dbConn;

                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = @"Delete from DimDateTime";
                
                // exec query
                result = sqlCmd.ExecuteNonQuery();

                // close connection 
                dbConn.Close();
            }

            return result;
        }

        public static long GetTableRowCount(string dbConnString, string tableName)
        {
            long result = 0;
            using (SqlConnection dbConn = new SqlConnection(dbConnString))
            {
                SqlCommand sqlCmd = new SqlCommand();

                // open connection 
                dbConn.Open();

                sqlCmd.Connection = dbConn;

                sqlCmd.CommandType = CommandType.Text;
                sqlCmd.CommandText = $@"SELECT COUNT(*) as [RowCount] FROM [{tableName}];";

                // exec query
                result = long.Parse(sqlCmd.ExecuteScalar().ToString());

                // close connection 
                dbConn.Close();
            }

            return result;
        }
    }
}
