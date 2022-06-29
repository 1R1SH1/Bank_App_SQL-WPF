using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_App_SQL_WPF
{
    internal class InfoLog
    {
        public List<string> log = new();

        public void AddToLog(string msg) => log.Add(msg);
    }
}
