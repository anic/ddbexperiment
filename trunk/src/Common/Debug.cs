using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace DistDBMS.Common
{
    public class Debug
    {
        public static void WriteLine(string s)
        {
            StackTrace st = new StackTrace(true);
            StackFrame frame = st.GetFrame(1);
            MethodBase method = frame.GetMethod();
            
            Console.WriteLine(DateTime.Now.ToString("hh:mm:ss.fff") + " " + method.DeclaringType.Name + "." + method.Name + ":" + s);
        }
    }
}
