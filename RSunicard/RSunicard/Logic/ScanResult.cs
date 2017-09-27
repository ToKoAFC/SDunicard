using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSunicard.Logic
{
    public class ScanResult
    {
        public string WorkerName { get; set; }
        public string EventType { get; set; }
        public bool CardIdExisted { get; set; }
    }
}
