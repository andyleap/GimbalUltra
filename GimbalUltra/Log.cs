using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GimbalUltra
{
    public class Log
    {
        static StreamWriter logfile = new StreamWriter("GimbalUltra.log");

        public static void Debug(string message, params object[] Args)
        {
            logfile.WriteLine(string.Format(message, Args));
            logfile.Flush();
        }

        public static void Exception(Exception e)
        {
            logfile.Write(e.ToString());
            logfile.Flush();
        }
    }
}
