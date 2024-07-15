using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KDMSViewer.Model
{
    public class ProgramHandler
    {
        public static int GetProcID(string proc_name)
        {
            int procid = 0;
            Process[] proList = Process.GetProcessesByName(proc_name);
            foreach (var proc in proList)
            {
                procid = proc.Id;
            }
            return procid;
        }
    }
}
