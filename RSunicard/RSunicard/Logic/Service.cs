﻿using Newtonsoft.Json;
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
        // DATABASE SERVICE
        static readonly string DBpath = @"C:\\RSunicard";


        public static DBModel GetDBModel()
        {
            var fileName = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_database.txt";
            var database = new DBModel();

            if (File.Exists($"{DBpath}\\{fileName}"))
            {
                try
                {
                    var dbJson = File.ReadAllText($"{DBpath}\\{fileName}");
                    var dbModel = JsonConvert.DeserializeObject<DBModel>(dbJson);
                    database = dbModel ?? new DBModel();
                }
                catch { }
            }
            return database;
        }

        public static List<EventVM> GetTodaysEvents()
        {
            var dBModel = GetDBModel();
            if (dBModel.Companies == null || !dBModel.Companies.Any())
            {
                return new List<EventVM>();
            }
            var events = dBModel.Companies.SelectMany(c => c.Workers.SelectMany(w => w.Events.Select(e => new EventVM
            {
                CardID = w.CardID,
                CompanyName = c.CompanyName,
                EventDate = e.EventDate.ToLocalTime().ToString("dd MM yyyy HH:mm:ss"),
                SortParam = e.EventDate,
                EventType = e.EventType,
                WorkerName = w.WorkerName
            }))).OrderByDescending(x => x.SortParam).ToList();
            return events;
        }

        public static List<CompanyVM> GetCompanyList()
        {
            var dbModel = GetDBModel();
            var result = new List<CompanyVM>();
            if (dbModel.Companies != null)
            {
                result = dbModel.Companies.Select(x => new CompanyVM
                {
                    CompanyName = x.CompanyName,
                    WorkersCount = x.Workers.Where(w => w.Events.Last().EventType == "Wejscie").ToList().Count
                }).ToList();
                result.Add(new CompanyVM
                {
                    CompanyName = "Lista wszystkich",
                    WorkersCount = dbModel.Companies.SelectMany(x => x.Workers.Where(v => v.Events.Last().EventType == "Wejscie")).ToList().Count
                });
            };
            return result;
        }

        public static List<WorkerVM> GetPresentWorkers(string companyName)
        {
            var dbModel = GetDBModel();
            var company = dbModel.Companies.Where(c => c.CompanyName == companyName).FirstOrDefault();
            if (company == null)
            {
                return new List<WorkerVM>();
            }
            var workers = company.Workers.Where(w => w.Events.Last().EventType == "Wejscie").Select(w => new WorkerVM
            {
                CardID = w.CardID,
                Name = w.WorkerName
            }).ToList();
            return workers;
        }

        public static bool CheckCardIdIfExist(string cardId)
        {
            var dbModel = GetDBModel();
            var companies = dbModel.Companies.ToList();
            if (companies == null || !companies.Any())
            {
                return false;
            }
            var result = companies.SelectMany(c => c.Workers.Select(w => w.CardID)).Where(c => c == cardId).Any();
            return result;
        }













        public static void SaveDatabase(DBModel database)
        {
            var fileName = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_database.txt";
            if (!Directory.Exists(DBpath))
            {
                Directory.CreateDirectory(DBpath);
            }
            if (!File.Exists($"{DBpath}\\{fileName}"))
            {
                File.CreateText($"{DBpath}\\{fileName}");
            }
            try
            {
                var json = new JavaScriptSerializer().Serialize(database);
                File.WriteAllText($"{DBpath}\\{fileName}", json);
            }
            catch { }
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
                        EventType = "Dodanie karty"
                    }
                }
            });
            SaveDatabase(dbModel);
        }

        public static void DeleteCompany(string companyName)
        {
            var dbModel = GetDBModel();
            var company = dbModel.Companies.Where(x => x.CompanyName == companyName).FirstOrDefault();
            dbModel.Companies.Remove(company);
            SaveDatabase(dbModel);
        }

        public static void DeleteWorker(string cardId)
        {
            var dbModel = GetDBModel();

            var workerToDelete = dbModel.Companies.SelectMany(c => c.Workers).Where(w => w.CardID == cardId);
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

        public static List<WorkerVM> GetWorkersSelectList()
        {
            var dbModel = GetDBModel();
            var result = new List<WorkerVM>();
            if (dbModel.Companies != null)
            {
                result = dbModel.Companies.SelectMany(x => x.Workers).Select(c =>
                new WorkerVM
                {
                    CardID = c.CardID,
                    Name = c.WorkerName
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
                        Name = x.WorkerName
                    }).ToList()
                }).FirstOrDefault();
            };
            return result;
        }

        public static ScanResult AddEventToWorker(string cardId)
        {
            if (cardId.Length != 8)
            {
                return new ScanResult { CardIdExisted = false };
            }
            var dbModel = GetDBModel();
            var worker = dbModel.Companies.SelectMany(x => x.Workers).FirstOrDefault(x => x.CardID == cardId);
            if (worker == null)
            {
                return new ScanResult { CardIdExisted = false };
            }
            var lastEvent = worker.Events.OrderBy(x => x.EventDate).LastOrDefault();
            switch (lastEvent.EventType)
            {
                case "Wejscie":
                    worker.Events.Add(new DBEvent
                    {
                        EventDate = DateTime.Now,
                        EventType = "Wyjscie"
                    });
                    SaveDatabase(dbModel);
                    return new ScanResult { CardIdExisted = true, EventType = "Wyjscie", WorkerName = worker.WorkerName };
                case "Wyjscie":
                case "Dodanie karty":
                    worker.Events.Add(new DBEvent
                    {
                        EventDate = DateTime.Now,
                        EventType = "Wejscie"
                    });
                    SaveDatabase(dbModel);
                    return new ScanResult { CardIdExisted = true, EventType = "Wejscie", WorkerName = worker.WorkerName };
                default:
                    return new ScanResult { CardIdExisted = false };
            }
        }

        public static void CreateDatebase()
        {
            var fileName = $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}_database.txt";
            if (!Directory.Exists(DBpath))
            {
                Directory.CreateDirectory(DBpath);
            }
            if (!File.Exists($"{DBpath}\\{fileName}"))
            {
                string[] fileEntries = Directory.GetFiles(DBpath);
                var lastDbPath = fileEntries.OrderBy(x => x).ToList().LastOrDefault();
                if (fileEntries.Length > 0)
                {
                    var dbJson = File.ReadAllText(lastDbPath, Encoding.GetEncoding("iso-8859-1"));
                    var dbModel = JsonConvert.DeserializeObject<DBModel>(dbJson);
                    var newdbModel = new DBModel
                    {
                        Companies = dbModel.Companies.Select(x => new DBCompany
                        {
                            CompanyName = x.CompanyName,
                            Workers = x.Workers.Select(w => new DBWorker
                            {
                                CardID = w.CardID,
                                WorkerName = w.WorkerName,
                                Events = new List<DBEvent>()
                                {
                                    w.Events.LastOrDefault()
                                }
                            }).ToList()
                        }).ToList()
                    };
                    var json = new JavaScriptSerializer().Serialize(newdbModel);
                    using (var sw = File.CreateText($"{DBpath}\\{fileName}"))
                    {
                        sw.WriteLine(json);
                    }
                    //File.WriteAllText($"{DBpath}\\{fileName}", json);
                }

            }
        }

    }
}
