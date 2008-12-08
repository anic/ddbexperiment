using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DistDBMS.UserInterface
{
    class LogWriter
    {
        string filename = "error.log";
        string spliter = "========================";
        public void WriteLog(string message)
        {
            string date = DateTime.Now.ToLongTimeString();


            FileStream fs = new FileStream(filename, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            sw.WriteLine(spliter + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + spliter);
            sw.WriteLine(message);
            sw.WriteLine(spliter + spliter);

            sw.Close();
            fs.Close();

        }
    }
}
