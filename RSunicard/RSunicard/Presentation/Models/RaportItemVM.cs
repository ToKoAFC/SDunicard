using System;

namespace RSunicard.Models
{
    public class RaportItemVM
    {
        public DateTime EventDate { get; set; }
        public string EventType { get; set; }
        public string WorkerName { get; set; }
        public string CompanyName { get; set; }
        public string WorkerCardID { get; set; }
    }
}
