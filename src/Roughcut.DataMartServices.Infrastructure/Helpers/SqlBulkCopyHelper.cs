using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; //using System.Data.SqlClient;


namespace Roughcut.DataMartServices.Infrastructure.Helpers
{
    public static class SqlBulkCopyHelper
    {

        static SqlBulkCopyHelper()
        {
            // tbd
        }

        public static void CopyDataTableToSourceTableWithColumnMappings(string dbConnString
                                , DataTable sourceDataTable
                                , string targetTableName
                                , List<DataTransferColumnMapping> columnMappings)
        {

            using (SqlConnection sqlDbConnection = new SqlConnection(dbConnString))
            {

                // Create the SqlBulkCopy object.  
                // Note that the column positions in the source DataTable  
                // match the column positions in the destination table so  
                // there is no need to map columns.  
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlDbConnection))
                {
                    // set target-sql-tableName
                    sqlBulkCopy.DestinationTableName = targetTableName;

                    // tweak properties
                    sqlBulkCopy.BatchSize = 10000;
                    sqlBulkCopy.NotifyAfter = 10000;
                    //bulkCopy.EnableStreaming = true;

                    // setup datatable columns
                    foreach (DataTransferColumnMapping mapping in columnMappings)
                    {
                        // Set up the column mappings by name.
                        var tempColumnMapping =
                            new SqlBulkCopyColumnMapping(mapping.SourceColumnName, mapping.TargetColumnName);

                        sqlBulkCopy.ColumnMappings.Add(tempColumnMapping);
                    }

                    try
                    {
                        // open db connection 
                        sqlDbConnection.Open();

                        // Write from the source to the destination.
                        sqlBulkCopy.WriteToServer(sourceDataTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                    //// -*******************************************************************
                    //// new, to be added - Houston - 09.07.15
                    //// -*******************************************************************


                    //// Set up the column mappings by name.
                    //SqlBulkCopyColumnMapping mapID =
                    //    new SqlBulkCopyColumnMapping("ProductID", "ProdID");

                    //bulkCopy.ColumnMappings.Add(mapID);

                    //SqlBulkCopyColumnMapping mapName =
                    //    new SqlBulkCopyColumnMapping("Name", "ProdName");
                    //bulkCopy.ColumnMappings.Add(mapName);

                    //SqlBulkCopyColumnMapping mapMumber =
                    //    new SqlBulkCopyColumnMapping("ProductNumber", "ProdNum");
                    //bulkCopy.ColumnMappings.Add(mapMumber);

                    //// Write from the source to the destination. 
                    //try
                    //{
                    //    bulkCopy.WriteToServer(reader);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    //finally
                    //{
                    //    // Close the SqlDataReader. The SqlBulkCopy 
                    //    // object is automatically closed at the end 
                    //    // of the using block.
                    //    reader.Close();
                    //}
                }
            }

        }

        public static void CopyDataTableToSourceTable(string dbConnString
            , DataTable sourceDataTable
            , string targetTableName)
        {

            using (SqlConnection sqlDbConnection = new SqlConnection(dbConnString))
            {

                // Create the SqlBulkCopy object.  
                // Note that the column positions in the source DataTable  
                // match the column positions in the destination table so  
                // there is no need to map columns.  
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlDbConnection))
                {
                    // set target-sql-tableName
                    sqlBulkCopy.DestinationTableName = targetTableName;

                    // tweak properties
                    sqlBulkCopy.BatchSize = 10000;
                    //bulkCopy.EnableStreaming = true;

                    try
                    {
                        // open db connection 
                        sqlDbConnection.Open();
                        
                        // Write from the source to the destination.
                        sqlBulkCopy.WriteToServer(sourceDataTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                    //// -*******************************************************************
                    //// new, to be added - Houston - 09.07.15
                    //// -*******************************************************************


                    //// Set up the column mappings by name.
                    //SqlBulkCopyColumnMapping mapID =
                    //    new SqlBulkCopyColumnMapping("ProductID", "ProdID");
                    
                    //bulkCopy.ColumnMappings.Add(mapID);

                    //SqlBulkCopyColumnMapping mapName =
                    //    new SqlBulkCopyColumnMapping("Name", "ProdName");
                    //bulkCopy.ColumnMappings.Add(mapName);

                    //SqlBulkCopyColumnMapping mapMumber =
                    //    new SqlBulkCopyColumnMapping("ProductNumber", "ProdNum");
                    //bulkCopy.ColumnMappings.Add(mapMumber);

                    //// Write from the source to the destination. 
                    //try
                    //{
                    //    bulkCopy.WriteToServer(reader);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine(ex.Message);
                    //}
                    //finally
                    //{
                    //    // Close the SqlDataReader. The SqlBulkCopy 
                    //    // object is automatically closed at the end 
                    //    // of the using block.
                    //    reader.Close();
                    //}
                }
            }

        }
    }
}
