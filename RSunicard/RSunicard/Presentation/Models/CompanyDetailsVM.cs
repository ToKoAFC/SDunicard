using System.Collections.Generic;

namespace RSunicard.Models
{
    public class CompanyDetailsVM
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<WorkerVM> Workers { get; set; }
    }
}
