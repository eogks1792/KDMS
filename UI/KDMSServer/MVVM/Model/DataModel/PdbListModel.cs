using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KDMSServer.Model
{
    public class PdbListModel
    {
        public int PdbId { get; set; }
        public string PdbName { get; set; } = string.Empty;
        public string PdbMd5 { get; set; } = string.Empty;
        public bool IsModify { get; set; }
    }
}
