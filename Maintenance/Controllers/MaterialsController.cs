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
    public class MaterialsController : Controller
    {
        MaterialsEntityClass materialsEntity = new MaterialsEntityClass();
        public int productId;
        [HttpGet]
        public ActionResult Create(int Product)
        {
            ViewBag.ProductId = Product;
            var accessories = materialsEntity.GetAccessories(Product);
            return View(accessories);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            for (int i = 1; i < collection.Count; i++)
            {
                productId = materialsEntity.Create(Convert.ToInt32(collection[i]), Convert.ToInt32(collection[0]));
            }
            return RedirectToAction("Materials", "Main", new
            {
                ProductId = productId
            });
        }

        [HttpGet]
        public ActionResult Delete(int id, int ProductID)
        {
            var materials = materialsEntity.GetMaterials(id, ProductID);
            return View(materials);
        }
        [HttpPost]
        public ActionResult Delete(Materials materials)
        {
            materialsEntity.Delete(materialsEntity.GetMaterials(materials.AccessoryId, materials.ProductId));
            return RedirectToAction("Materials", "Main", new { materials.ProductId });
        }
    }
}