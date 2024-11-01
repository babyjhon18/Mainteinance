using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.ViewModels
{
    public class RepairViewModel : RepairClass
    {
        public Clients ClientsToExport { get; set; }
        public IEnumerable<ResponsiblePerson> Responsibles { get; set; }
        public new IEnumerable<JobsViewModel> Jobs { get; set; }
        public new IEnumerable<AccessoryViewModel> Accessories { get; set; }
        public ResponsiblePersonsProperties personProperties { get; set; }
    }
}