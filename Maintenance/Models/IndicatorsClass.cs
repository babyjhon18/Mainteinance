using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class IndicatorsClass
    {
        public List<ResponsiblePersonsProperties> ResponsiblePersonsProperties { get; set; }
        public List<BaseIndicatorModel> BaseIndicatorModel { get; set; }
        public IndicatorsClass() 
        {
            ResponsiblePersonsProperties = new List<ResponsiblePersonsProperties>();
            BaseIndicatorModel = new List<BaseIndicatorModel>();
        }
    }
}
