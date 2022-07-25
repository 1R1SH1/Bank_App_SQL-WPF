using System.Collections.Generic;

namespace Bank_App_SQL_WPF
{
    internal class InfoLog
    {
        public List<string> log = new();

        public void AddToLog(string msg) => log.Add(msg);
    }
}
