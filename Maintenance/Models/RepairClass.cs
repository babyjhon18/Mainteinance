using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Models
{
    public class RepairClass
    {
        DataBaseContext context = new DataBaseContext();
        public RepairClass()
        {
            Clients = context.Clients;
            Jobs = context.Jobs;
            Materials = context.Materials;
            Accessories = context.Accessories;
            Products = context.Products;
            var _acc = from accessory in Accessories
                       orderby accessory.Name
                       join material in Materials on accessory.Id equals material.AccessoryId
                       join product in Products on material.ProductId equals product.Id
                       where accessory.isDeleted == false
                       select new Accessories() { Id = accessory.Id, Name = accessory.Name, 
                           Value = accessory.Value, MeasureUnit = accessory.MeasureUnit, isDeleted = accessory.isDeleted };
            Accessories = _acc.ToList();
            AccessoriesForEditJobs = context.Accessories.ToList().Where(ac => ac.isDeleted == false);
            ResponsiblePersonsProperties = context.ResponsiblePersonsProperties;
        }
        public Repairs Repairs { get; set; }
        public IEnumerable<Clients> Clients { get; set; }
        public IEnumerable<Jobs> Jobs { get; set; }
        public IEnumerable<Accessories> Accessories { get; set; }
        public IEnumerable<Accessories> AccessoriesForEditJobs { get; set; }
        public IEnumerable<Materials> Materials { get; set; }
        public IEnumerable<Products> Products { get; set; }
        public IEnumerable<ResponsiblePersonsProperties> ResponsiblePersonsProperties { get; set; }
    }
}