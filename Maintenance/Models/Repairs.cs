using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class Repairs
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public string FactoryNumber { get; set; }
        public string RTUNumber { get; set; }
        public DateTime DateToRepair { get; set; }
        public string Defect { get; set; }
        public string Set { get; set; }
        public string Jobs { get; set; }
        public string Materials { get; set; }
        public string Devices { get; set; }
        public DateTime DateToClient { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string Acct { get; set; }
        public string AcctNumber { get; set; }
        public DateTime DateToAcct { get; set; }
        public bool Deleted { get; set; }
        public string ContactPerson { get; set; }
        public int ResponsiblePersonId { get; set; }
        public DateTime Deadline { get; set; }

    }
}