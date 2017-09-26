using System.Collections.Generic;

namespace RSunicard.Database.Models
{
    public class DBWorker
    {
        public string CardID { get; set; }
        public string WorkerName { get; set; }
        public List<DBEvent> Events { get; set; }
    }
}
