using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class Clients
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CheckingAccount { get; set; }
        public string Bank { get; set; }
        public string BankAddress { get; set; }
        public string BIC { get; set; }
        public string UNP { get; set; }
        public string OKPO { get; set; }
        public string ResponsiblePerson { get; set; }
        public string ContactNumber { get; set; }
        public string RPJobTitle { get; set; }
        public string ContractNumber { get; set; }
        public string ContractDate { get; set; }
        public string LongResponsiblePerson { get; set; }
        public string BasedOnDescription { get; set; }
        public string Email { get; set; }
    }
}