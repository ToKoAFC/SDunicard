using Newtonsoft.Json;
using RSunicard.Database.Models;
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
    }
}
