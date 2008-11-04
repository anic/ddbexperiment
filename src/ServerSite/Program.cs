using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace DistDBMS.ControlSite
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source = DDBTest";
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = conn;
            cmd.CommandType = System.Data.CommandType.Text;
            //cmd.CommandText = "CREATE  TABLE 'main'.'Student' ('key' INTEGER PRIMARY KEY  NOT NULL )";

            //int result = cmd.ExecuteNonQuery();
            //System.Console.WriteLine(result.ToString());

            cmd.CommandText = "select * from student";
            SQLiteDataReader reader = cmd.ExecuteReader();
            System.Console.WriteLine(reader[1].ToString());
        }
    }
}
