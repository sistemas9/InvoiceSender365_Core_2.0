﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace serviceTest.TestHelper
{
    class database
    {
        IDataReader rd = null;
        int rowsAfected = 0;
        public SqlConnection conn;

        public database()
        {
            var AppSettings = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

            String conectionString = AppSettings["DB_AYTPROD"];
            // conn = new SqlConnection(Environment.GetEnvironmentVariable("DB_AYTPROD").ToString());
            conn = new SqlConnection();
            conn.Close();
        }

        public IDataReader query(String queryStr)
        {
            SqlCommand command = new SqlCommand(queryStr, conn);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            try
            {
                rd = command.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rd;
        }

        public int queryInsert(String queryStr)
        {
            SqlCommand command = new SqlCommand(queryStr, conn);
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            try
            {
                rowsAfected = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return rowsAfected;
        }
    }
}
