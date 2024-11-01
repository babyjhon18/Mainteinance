using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public class ResponsiblePersonsProperties
    {
        public int id { get; set; }
        public string FullName { get; set; }
        public double sellaryPerMonth { get; set; }
        public double avgWorkTime { get; set; }
        public double hourPrice { get; set; }
    }
}
