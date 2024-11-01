using Project.Domain;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class AccessoriesController : Controller
    {
        AccessorieEntityClass accessorieEntity = new AccessorieEntityClass();

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string AccessoriesName, string AccessoriesValue, string AccessoriesMeasureUnit)
        {
            var lastCreated = accessorieEntity.Create(AccessoriesName, AccessoriesValue, AccessoriesMeasureUnit);
            Session["LastUpdatedOrCreatedAccessory"] = lastCreated;
            Session["ShowDeleted"] = false;
            return RedirectToAction("Accessories", "Main");
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            var accessories = accessorieEntity.GetAccessorie(id, false);
            return View(accessories);
        }
        [HttpPost]
        public ActionResult Update(Accessories accessories)
        {
            var lastUpdated = accessorieEntity.Update(accessories);
            Session["LastUpdatedOrCreatedAccessory"] = lastUpdated;
            Session["ShowDeleted"] = false;
            return RedirectToAction("Accessories", "Main");
        }
        [HttpGet]
        public ActionResult Restore(int id)
        {
            var accessories = accessorieEntity.GetAccessorie(id, true);
            return View(accessories);
        }
        [HttpPost]
        public ActionResult Restore(Accessories accessories)
        {
            var lastRestored = accessorieEntity.Restore(accessories);
            Session["ShowDeleted"] = false;
            Session["LastUpdatedOrCreatedAccessory"] = lastRestored;
            return RedirectToAction("Accessories", "Main");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var accessories = accessorieEntity.GetAccessorie(id, true);
            return View(accessories);
        }
        [HttpPost]
        public ActionResult Delete(Accessories accessories)
        {
            accessorieEntity.Delete(accessories);
            Session["ShowDeleted"] = false;
            return RedirectToAction("Accessories", "Main");
        }
    }
}