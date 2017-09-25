using System.Collections.Generic;

namespace RSunicard.Database.Models
{
    public class DBWorker
    {
        public string CardID { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public List<DBEvent> Events { get; set; }
    }
}
