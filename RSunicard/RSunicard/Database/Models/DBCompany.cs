using System.Collections.Generic;

namespace RSunicard.Database.Models
{
    public class DBCompany
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public List<DBWorker> Workers { get; set; }
    }
}
