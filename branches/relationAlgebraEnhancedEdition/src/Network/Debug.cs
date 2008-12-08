using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;

namespace DistDBMS.Network
{
    public class Debug
    {
        public static void WriteLine(string s)
        {
            StackTrace st = new StackTrace(true);
            StackFrame frame = st.GetFrame(1);
            MethodBase method = frame.GetMethod();
            Console.WriteLine("[DEBUG] " + method.DeclaringType.Name + "." + method.Name + ":" + s);
        }
    }
}
