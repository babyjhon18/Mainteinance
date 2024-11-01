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
    public class ProductsController : Controller
    {
        ProductEntityClass productEntity = new ProductEntityClass();

        [HttpGet]
        public ActionResult Create()
        {
            var accessories = productEntity.GetProduct();
            return View(accessories);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            productEntity.Create(form);
            return RedirectToAction("ProductsList", "Main");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Delete(Products products)
        {
            productEntity.Delete(products);
            return RedirectToAction("ProductsList", "Main");
        }
    }
}