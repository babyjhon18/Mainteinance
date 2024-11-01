using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project.Domain
{
    public class ProductEntityClass : BaseEntityClass
    {
        public IEnumerable<Accessories> GetProduct()
        {
            IEnumerable<Accessories> accessories = dataBase.Accessories;
            return accessories;
        }
        public void Create(Dictionary<string, string> form)
        {
            Products products = new Products
            {
                Name = form.ElementAt(0).Value,
            };
            dataBase.Products.Add(products);
            dataBase.SaveChanges();
            for (int i = 1; i < form.Count; i++)
            {
                Accessories accessories = dataBase.Accessories.Find(Convert.ToInt32(form.ElementAt(i).Value));
                Materials materials = new Materials
                {
                    Id = accessories.Id,
                    AccessoryId = accessories.Id,
                    ProductId = products.Id
                };
                dataBase.Materials.Add(materials);
            }
            dataBase.SaveChanges();
        }

        public void Delete(Products products)
        {
            var mater = dataBase.Materials.Where(material => material.ProductId == products.Id).ToArray();
            foreach(var m in mater)
            {
                Delete(m as object);
            }
            Delete(products as object);
        }
    }
}