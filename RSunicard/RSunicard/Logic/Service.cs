using Newtonsoft.Json;
using RSunicard.Database.Models;
using RSunicard.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RSunicard.Logic
{
    public static class Service
    {
        static readonly string DBpath = @"C:\\RSunicard";
        static readonly string DBfilename = @"RSunicard.txt";

        public static DBModel GetDBModel()
        {
            var database = new DBModel();

            if (File.Exists($"{DBpath}\\{DBfilename}"))
            {
                try
                {
                    var dbJson = File.ReadAllText($"{DBpath}\\{DBfilename}");
                    var dbModel = JsonConvert.DeserializeObject<DBModel>(dbJson);
                    database = dbModel ?? new DBModel();
                }
                catch { }
            }
            return database;
        }

        public static void SaveDatabase(DBModel database)
        {
            if (!Directory.Exists(DBpath))
            {
                Directory.CreateDirectory(DBpath);
            }
            if (!File.Exists($"{DBpath}\\{DBfilename}"))
            {
                File.CreateText($"{DBpath}\\{DBfilename}");
            }
            try
            {
                var json = new JavaScriptSerializer().Serialize(database);
                File.WriteAllText($"{DBpath}\\{DBfilename}", json);
            }
            catch { }
        }

        public static List<EventVM> GetEvents(string companyName)
        {
            var dBModel = GetDBModel();
            if (dBModel.Companies == null || !dBModel.Companies.Any())
            {
                return new List<EventVM>();
            }
            var company = dBModel.Companies.FirstOrDefault(x => x.CompanyName == companyName);
            if (company == null)
            {
                return new List<EventVM>();
            }
            return company.Workers.SelectMany(w => w.Events.Select(e => new EventVM
            {
                CardID = w.CardID,
                CompanyName = company.CompanyName,
                EventDate = e.EventDate.ToLocalTime().ToString("dd MM yyyy HH:mm:ss"),
                EventType = e.EventType,
                WorkerName = w.WorkerName
            })).ToList();
        }

        public static void AddNewCompany(string companyName)
        {
            var dbModel = GetDBModel();
            dbModel.Companies.Add(new DBCompany
            {
                CompanyName = companyName,
                Workers = new List<DBWorker>()
            });
            SaveDatabase(dbModel);
        }

        public static void AddNewWorker(string companyName, string userName, string cardId)
        {
            var dbModel = GetDBModel();
            var company = dbModel.Companies.Where(x => x.CompanyName == companyName).FirstOrDefault();
            company.Workers.Add(new DBWorker
            {
                CardID = cardId,
                WorkerName = userName,
                Events = new List<DBEvent>()
                {
                    new DBEvent
                    {
                        EventDate = DateTime.Now,
                        EventType = "Wejscie"
                    }
                }
            });
            SaveDatabase(dbModel);
        }

        public static List<CompanyVM> GetCompanySelectList()
        {
            var dbModel = GetDBModel();
            var result = new List<CompanyVM>();
            if (dbModel.Companies != null)
            {
                result = dbModel.Companies.Select(x => new CompanyVM
                {
                    CompanyName = x.CompanyName,
                    WorkersCount = x.Workers.Count
                }).ToList();
            };
            return result;
        }

        public static CompanyDetailsVM GetCompanyDetails(string companyName)
        {
            var dbModel = GetDBModel();
            var result = new CompanyDetailsVM();
            if (dbModel.Companies != null)
            {
                result = dbModel.Companies.Where(c => c.CompanyName == companyName).Select(c => new CompanyDetailsVM
                {
                    CompanyName = c.CompanyName,
                    Workers = c.Workers.Select(x => new WorkerVM
                    {
                        CardID = x.CardID,
                        FirstName = x.WorkerName
                    }).ToList()
                }).FirstOrDefault();
            };
            return result;
        }

        public static bool ReceiveSerialPortSignal(string input)
        {
            if (input.Length != 8)
            {
                return false;
            }
            var dbModel = GetDBModel();
            var worker = dbModel.Companies.SelectMany(x => x.Workers).FirstOrDefault(x => x.CardID == input);
            if (worker == null)
            {
                return false;
            }
            var lastEvent = worker.Events.OrderBy(x=> x.EventDate).LastOrDefault();
            switch (lastEvent.EventType)
            {
                case "Wejscie":
                    worker.Events.Add(new DBEvent
                    {
                        EventDate = DateTime.Now,
                        EventType = "Wyjscie"
                    });
                    SaveDatabase(dbModel);
                    return true;
                case "Wyjscie":
                    worker.Events.Add(new DBEvent
                    {
                        EventDate = DateTime.Now,
                        EventType = "Wejscie"
                    });
                    SaveDatabase(dbModel);
                    return true;
                default:
                    return false;
            }
        }

        public static void CreateDatebase()
        {
            if (!Directory.Exists(DBpath))
            {
                Directory.CreateDirectory(DBpath);
            }
            if (!File.Exists($"{DBpath}\\{DBfilename}"))
            {
                File.CreateText($"{DBpath}\\{DBfilename}");
            }
        }
    }
}
