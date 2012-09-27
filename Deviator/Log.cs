using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Deviator
{
    public delegate void Logable(string str);
    public static class Log
    {
        public static List<Logable> Logables = new List<Logable>();
        public static void STR(string str)
        {
            string line;
            StreamWriter sr = new StreamWriter((Stream)File.Open("Deviator.log", FileMode.Append));
            line = "[" + DateTime.Now.ToShortDateString() + "][" + DateTime.Now.ToShortTimeString() + "]\t" + str;
            sr.WriteLine(line);
            sr.Close();
            for (int i = 0; i < Logables.Count; ++i)
            {
                if (Logables[i] != null)
                    Logables[i](line);
            }
        }
    }
}
