using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BackgroundService
{
    public static class Logger
    {
        public static string _Path = "C:\\Log";
        public static void WriteLog(Exception e)
        {
            if (Directory.Exists(_Path))
            {
                using (StreamWriter sw = new StreamWriter(Path.Combine(_Path, "BackgroundService.log"), true))
                {
                    sw.WriteLine("[" + DateTime.Now.ToString() + "] Исключение \"" + e.Message + "\" в области: " + e.TargetSite.Name + "; stacktrace: " + e.StackTrace + ";");
                }
            }
            else
            {
                throw new DirectoryNotFoundException("Path of \"" + _Path + "\" not found!");
            }
        }
    }
}