using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Domain
{
    public class MaterialsEntityClass : BaseEntityClass
    {
        Materials materials = new Materials();
        public IEnumerable<Accessories> GetAccessories(int Product)
        {
            IEnumerable<Accessories> accessories = dataBase.Accessories;
            return accessories;
        }
        public int Create(int AccessorieID, int ProductId)
        {
            Accessories accessories = dataBase.Accessories.Find(AccessorieID);
            materials = new Materials
            {
                AccessoryId = accessories.Id,
                ProductId = Convert.ToInt32(ProductId)
            };
            dataBase.Materials.Add(materials);
            dataBase.SaveChanges();
            return materials.ProductId;
        }
        public Materials GetMaterials(int id, int ProductID)
        {
            Materials materials = dataBase.Materials.Where(m => m.ProductId == ProductID).Where(m => m.AccessoryId == id).FirstOrDefault();
            return materials;
        }
        public void Delete(Materials materials)
        {
            Delete(materials as object);
        }
    }
}