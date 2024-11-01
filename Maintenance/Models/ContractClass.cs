using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class ContractClass
    {
        public IEnumerable<ResponsiblePerson> Person { get; set; }
        public Clients Clients { get; set; }

        public ContractClass()
        {
            Clients = new Clients();
            Person = new List<ResponsiblePerson>();
        }
    }
}