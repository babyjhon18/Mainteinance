using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class Materials
    {
        public int Id { get; set; }
        public int AccessoryId { get; set; }
        public int ProductId { get; set; }
        public virtual IQueryable<Accessories> Accessories { get; set; }
    }
}