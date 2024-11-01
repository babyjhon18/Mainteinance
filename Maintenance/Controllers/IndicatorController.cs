using Project.Domain;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class IndicatorController : Controller
    {
        public IndicatorEntityClass indicatorEntity = new IndicatorEntityClass();
        public ResponsiblePersonsPropertiesEntityClass responsibleEntity = new ResponsiblePersonsPropertiesEntityClass();
        // GET: Indicator
        public ActionResult Index()
        {
            return View(responsibleEntity.GetResponsiblePersonProperties());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            indicatorEntity.Create(form);
            return RedirectToAction("Indicators", "Main");
        }

        [HttpGet]
        public ActionResult UpdatePerson(int id)
        {
            var person = responsibleEntity.GetPerson(id);
            return View(person);
        }
        [HttpPost]
        public ActionResult UpdatePerson(ResponsiblePersonsProperties person)
        {
            responsibleEntity.Update(person);
            return RedirectToAction("Indicators", "Main");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var person = responsibleEntity.GetPerson(id);
            return View(person);
        }
        [HttpPost]
        public ActionResult Delete(ResponsiblePersonsProperties person)
        {
            indicatorEntity.Delete(person);
            return RedirectToAction("Indicators", "Main");
        }
        [HttpPost]
        public ActionResult Update(FormCollection form)
        {
            indicatorEntity.Update("IndicatorConstants.json", form);
            return RedirectToAction("Indicators", "Main");
        }
    }
}