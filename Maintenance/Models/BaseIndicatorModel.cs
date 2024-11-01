using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class BaseIndicatorModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public string ValueDescription { get; set; }
    }
}