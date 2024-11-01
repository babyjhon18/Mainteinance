using Morpher.WebService.V2;
using Newtonsoft.Json;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Domain
{
    public class IndicatorEntityClass : BaseEntityClass
    {
        public void Create(Dictionary<string, string> collection)
        {
            ResponsiblePersonsProperties responsiblePersons = new ResponsiblePersonsProperties()
            {
                FullName = collection["FullName"].ToString(),
                sellaryPerMonth = Convert.ToDouble(collection["sellaryPerMonth"]),
                avgWorkTime = Convert.ToDouble(collection["avgWorkTime"]),
                hourPrice = Convert.ToDouble(collection["hourPrice"])
            };
            dataBase.ResponsiblePersonsProperties.Add(responsiblePersons);
            dataBase.SaveChanges();
        }
        public void Update(string file, FormCollection collection)
        {
            var json = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + file);
            var indicators = JsonConvert.DeserializeObject<IEnumerable<BaseIndicatorModel>>(json) as List<BaseIndicatorModel>;
            indicators[0].Value = double.Parse(collection[0], System.Globalization.CultureInfo.InvariantCulture) / 100; 
            indicators[1].Value = double.Parse(collection[1], System.Globalization.CultureInfo.InvariantCulture) / 100; 
            indicators[2].Value = double.Parse(collection[2], System.Globalization.CultureInfo.InvariantCulture) / 100;
            indicators[3].Value = double.Parse(collection[3], System.Globalization.CultureInfo.InvariantCulture) / 100;
            indicators[4].Value = double.Parse(collection[4], System.Globalization.CultureInfo.InvariantCulture) / 100;
            var content = JsonConvert.SerializeObject(indicators);
            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + file, content);
        }
    }
}