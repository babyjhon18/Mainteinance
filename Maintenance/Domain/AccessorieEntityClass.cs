using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project.Domain
{
    public class AccessorieEntityClass : BaseEntityClass
    {
        public Accessories GetAccessorie(int id, bool isDelete)
        {
            var accessories = dataBase.Accessories.Find(id);
            if (isDelete)
            {
                var repair = dataBase.Repairs.Where(r => r.Materials.Contains(accessories.Id.ToString())).FirstOrDefault();
                if(repair != null)
                {
                    accessories.Id = -1;
                    return accessories;
                }
                else
                {
                    return accessories;
                }
            }
            else 
                return accessories;
        }

        public int Create(string AccessoriesName, string AccessoriesValue, string AccessoriesMeasureUnit)
        {
            Accessories accessories = new Accessories
            {
                Name = AccessoriesName,
                Value = AccessoriesValue,
                MeasureUnit = AccessoriesMeasureUnit
            };
            dataBase.Accessories.Add(accessories);
            dataBase.SaveChanges();
            var lastAccessory = dataBase.Accessories.ToList().LastOrDefault();
            return lastAccessory.Id;
        }
        public int Update(Accessories accessories)
        {
            Update(accessories as object);
            return accessories.Id;
        }
       
        public void Delete(Accessories accessories)
        {
            Accessories accessoriesToDelete = dataBase.Accessories.Find(accessories.Id);
            accessoriesToDelete.isDeleted = true;
            Update(accessoriesToDelete as object);
            //Delete(accessories as object);
        }

        public int Restore(Accessories accessories)
        {
            Accessories accessoriesToDelete = dataBase.Accessories.Find(accessories.Id);
            accessoriesToDelete.isDeleted = false;
            Update(accessoriesToDelete as object);
            return accessories.Id;
        }
    }
}