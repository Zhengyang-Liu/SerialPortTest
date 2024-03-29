﻿using SerialPortTest.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SerialPortTest
{
    public class DatabaseManager
    {
        SqlConnection connection;

        public DatabaseManager(string serverName, string databaseName, string userName, string password)
        {
            string connectionString = string.Format("Data Source={0}; Initial Catalog={1}; User ID={2}; Password={3}", serverName, databaseName, userName, password);
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void BulkInsert(string tableName, List<DataStruct> dataList)
        {
            DataTable table = new DataTable("ParentTable");

            DataColumn timeColumn = new DataColumn();
            timeColumn.DataType = Type.GetType("System.String");
            timeColumn.ColumnName = "time";
            timeColumn.Unique = true;

            DataColumn accelerationColumn = new DataColumn();
            accelerationColumn.DataType = Type.GetType("System.Decimal");
            accelerationColumn.ColumnName = "acceleration";

            table.Columns.Add(timeColumn);
            table.Columns.Add(accelerationColumn);

            foreach (DataStruct item in dataList)
            {
                try
                {
                    DataRow row = table.NewRow();
                    row["time"] = item.dateTime;
                    row["acceleration"] = decimal.Parse(item.data);
                    table.Rows.Add(row);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "[" + tableName + "]";

                try
                {
                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(table);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public void CreateTable(string tableName)
        {
            string queryString =
            "CREATE TABLE [" + tableName + "] (" +
            "time DateTime NOT NULL,    " +
            "acceleration decimal(10,4) NOT NULL,    " +
            "PRIMARY KEY(time)); ";

            try
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(String.Format("SQL Command Excuted: {0}, {1}", reader[0], reader[1]));
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }

        internal void BulkInsert(Settings @default, List<DataStruct> dataList)
        {
            throw new NotImplementedException();
        }
    }
}
