using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Project.Domain
{
    public class BaseEntityClass
    {
        public DataBaseContext dataBase { get; set; }
        public BaseEntityClass()
        {
            dataBase = new DataBaseContext();
        }
        public void Update(Object objects)
        {
            dataBase.Entry(objects).State = EntityState.Modified;
            dataBase.SaveChanges();
        }
        public void Delete(Object objects)
        {
            dataBase.Entry(objects).State = EntityState.Deleted;
            dataBase.SaveChanges();
        }
    }
}