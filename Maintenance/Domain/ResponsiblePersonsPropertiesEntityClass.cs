using Newtonsoft.Json;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain
{
    public class ResponsiblePersonsPropertiesEntityClass : BaseEntityClass
    {
        public IEnumerable<BaseIndicatorModel> GetIndicators(string file)
        {
            var json = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "App_Data\\" + file);
            var indicators = JsonConvert.DeserializeObject<IEnumerable<BaseIndicatorModel>>(json) as List<BaseIndicatorModel>;
            indicators[3].Value = indicators[3].Value * 100;
            indicators[4].Value = indicators[4].Value * 100;
            indicators[5].Value = indicators[5].Value * 100;
            indicators[6].Value = indicators[6].Value * 100;
            indicators[7].Value = indicators[7].Value * 100;
            return indicators;
        }

        public ResponsiblePersonsProperties GetPerson(int id)
        {
            ResponsiblePersonsProperties person = dataBase.ResponsiblePersonsProperties.Find(id);
            return person;
        }

        public IndicatorsClass GetResponsiblePersonProperties()
        {
            List<ResponsiblePersonsProperties> personsProperties = new List<ResponsiblePersonsProperties>();
            foreach (var item in dataBase.ResponsiblePersonsProperties.AsEnumerable())
            {
                personsProperties.Add(item);
            }
            List<BaseIndicatorModel> indicators = GetIndicators("IndicatorConstants.json") as List<BaseIndicatorModel>;
            IndicatorsClass indicatorsClass = new IndicatorsClass() { BaseIndicatorModel = indicators, ResponsiblePersonsProperties = personsProperties };
            return indicatorsClass;
        }
    }
}

