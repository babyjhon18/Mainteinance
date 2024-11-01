using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Clients> Clients { get; set; }
        public DbSet<Materials> Materials { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<Accessories> Accessories { get; set; }
        public DbSet<Repairs> Repairs { get; set; }
        public DbSet<ResponsiblePerson> ResponsiblePersons { get; set; }
        public DbSet<ResponsiblePersonsProperties> ResponsiblePersonsProperties { get; set; }
    }
}