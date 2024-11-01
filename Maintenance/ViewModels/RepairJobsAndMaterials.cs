using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.ViewModels
{
    public class RepairJobsAndMaterials
    {
        public string ControllerName { get; set; }
        public NormsOfTime NormsOfTime { get; set; }
        public List<string> Accessories { get; set; }
        public List<string> Jobs { get; set; }
        public RepairJobsAndMaterials()
        {
            Accessories = new List<string>();
            Jobs = new List<string>();
            NormsOfTime = new NormsOfTime();
        }
    }

    public class NormsOfTime
    {
        public double InputControl { get; set; }
        public double SearchDefects { get; set; }
        public double RestoreDefects { get; set; }
        public double UpdateOS { get; set; }
        public double Check { get; set; }
    }

    public class ClientRepair
    {
        public string Client { get; set; }
        public string ClientContractNumber { get; set; }
        public string ClientLastContractDate { get; set; }
        public string ValueForJob { get; set; }
        public string Ids { get; set; }
        public ResponsiblePerson TResponsible { get; set; }
        public List<RepairJobsAndMaterials> RepairAndMaterials { get; set; }
        public ClientRepair()
        {
            TResponsible = new ResponsiblePerson();
            RepairAndMaterials = new List<RepairJobsAndMaterials>();
        }
    }
}