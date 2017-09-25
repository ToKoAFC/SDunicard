using System.Collections.Generic;

namespace RSunicard.Database.Models
{
    public class DBModel
    {
        public DBModel()
        {
            Companies = new List<DBCompany>();
        }
        public List<DBCompany> Companies { get; set; }
    }
}
