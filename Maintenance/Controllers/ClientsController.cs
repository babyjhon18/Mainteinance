using Project.Domain;
using Project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    public class ClientsController : Controller
    {
        ClientEntityClass clientEntity = new ClientEntityClass();
        String path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {   
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            clientEntity.Create(form);
            return RedirectToAction("Clients", "Main");
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            var clients = clientEntity.GetClient(id);
            return View(clients);
        }
        [HttpPost]
        public ActionResult Update(Clients clients)
        {
            clientEntity.Update(clients);
            return RedirectToAction("Clients", "Main");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var clients = clientEntity.GetClient(id);
            return View(clients);
        }
        [HttpPost]
        public ActionResult Delete(Clients clients)
        {
            clientEntity.Delete(clients);
            return RedirectToAction("Clients", "Main");
        }
        [HttpGet]
        public ActionResult Contract(int id)
        {
            var contractData = clientEntity.GetDataForContracts(id);
            return View(new ContractClass() { Clients = contractData.Clients, Person = contractData.Person });
        }
        [HttpPost]
        public ActionResult Contract(FormCollection clients)
        {
            var fileName = clientEntity.Contract(clients);
            return File(new FileStream(path + fileName, FileMode.Open), "application/docx", fileName);
        }
    }
}