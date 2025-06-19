using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using Project.Domain;
using Project.Models;

namespace Project.Controllers
{
    public class MainController : Controller
    {
        MainEntityClass mainEntity = new MainEntityClass();
        // GET: Main
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProductsList()
        {
            IEnumerable<Products> products = mainEntity.dataBase.Products;
            return View(products);
        }
        public ActionResult Clients()
        {
            IEnumerable<Clients> clients = mainEntity.dataBase.Clients;
            return View(clients);
        }
        public ActionResult _ViewClients(string condition, string key)
        {
            var clients = mainEntity.GetClients(condition, key);
            return View(clients);
        }
        public ActionResult Materials(int ProductId)
        {
            ViewBag.ProductId = ProductId;
            var products = mainEntity.GetProduct(ProductId);
            ViewBag.ProductName = products.Name;
            IEnumerable<Accessories> accessories = mainEntity.Materials(ProductId);
            accessories = accessories.OrderBy(ac => ac.Name);
            return View(accessories);
        }

        public ActionResult _ViewMaterials(string key, int ProductId)
        {
            IEnumerable<Accessories> accessories = mainEntity.GetMaterials(key, ProductId);
            Products products = mainEntity.GetProduct(ProductId);
            ViewBag.ProductName = products.Name;
            ViewBag.ProductID = products.Id;
            return View(accessories);
        }
        public ActionResult Jobs()
        {
            IEnumerable<Jobs> jobs = mainEntity.dataBase.Jobs;
            return View(jobs);
        }
        public ActionResult _ViewJobs(string key)
        {
            IEnumerable<Jobs> jobs = mainEntity.GetJobs(key);
            return View(jobs);
        }
        public ActionResult Accessories()
        {
            IEnumerable<Accessories> accessories = new List<Accessories>();
            if(Session["AccessoryKeyWord"] != null)
                accessories = mainEntity.GetAccessories(Session["AccessoryKeyWord"].ToString());
            else
                accessories = mainEntity.GetAccessories("");
            accessories = accessories.OrderBy(ac => ac.Name).ToList();
            Session["ShowDeleted"] = false;
            return View(accessories);
        }
        public ActionResult _ViewAccessories(string key, bool showDeleted)
        {
            Session["ShowDeleted"] = showDeleted;
            Session["AccessoryKeyWord"] = key;
            IEnumerable<Accessories> accessories = mainEntity.GetAccessories(key);
            return View(accessories);
        }
        public ActionResult Repair(string condition, string key, string dateFrom, string toDate,
            string selectedSort, string sortDirection)
        {
            DateTime dateFromBegin = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime toDateBegin = new DateTime(DateTime.Now.Year + 1, 12, 31);
            Session["dateFrom"] = dateFromBegin.ToString("d");
            Session["toDate"] = toDateBegin.ToString("d");
            IEnumerable<Repairs> repair = mainEntity.Repair(condition, key, dateFrom, toDate, selectedSort, sortDirection);
            var SortedRepairs = repair.OrderBy(r => r.DateToRepair).ToList();
            return View(SortedRepairs);
        }
        public ActionResult _ViewRepairs(string condition, string key, string dateFrom, string toDate, 
            string selectedSort, string sortDirection)
        {
            Session["Key"] = key;
            Session["Condition"] = condition;
            Session["dateFrom"] = dateFrom;
            Session["toDate"] = toDate;
            IEnumerable<Repairs> repair = mainEntity.GetRepairsV2(condition, key, dateFrom, toDate, selectedSort, sortDirection, false);
            return View(repair);
        }

        public ActionResult DeletedRepairs(string condition, string key, string dateFrom, string toDate,
            string selectedSort, string sortDirection)
        {
            IEnumerable<Repairs> repair = mainEntity.DeletedRepairs(condition, key, dateFrom, toDate, selectedSort, sortDirection);
            var SortedRepairs = repair.OrderBy(r => r.DateToRepair).ToList();
            return View(SortedRepairs);
        }
        public ActionResult _ViewDeletedRepairs(string condition, string key, string dateFrom, string toDate,
            string selectedSort, string sortDirection)
        {
            Session["Key"] = key;
            Session["Condition"] = condition;
            Session["dateFrom"] = dateFrom;
            Session["toDate"] = toDate;
            IEnumerable<Repairs> repair = mainEntity.GetRepairsV2(condition, key, dateFrom, toDate, selectedSort, sortDirection, true);
            return View(repair);
        }

        public ActionResult Indicators()
        {
            if (Request.HttpMethod == "POST")
            {
                string enteredPassword = Request.Form["password"];
                string correctPassword = ConfigurationManager.AppSettings["Indicator"];

                if (enteredPassword != correctPassword)
                {
                    ViewBag.IsAuthenticated = false;
                    return View(); // Показать форму повторно с сообщением об ошибке
                }
                else {
                    // Если пароль верный, получаем данные
                    IndicatorsClass indicators = mainEntity.GetResponsiblePersonProperties();
                    ViewBag.IsAuthenticated = true;
                    // Возвращаем вид
                    return View(indicators);
                }
            }
            return View();
        }

        public ActionResult _ViewIndicators()
        {
            IndicatorsClass indicators = mainEntity.GetResponsiblePersonProperties();
            return View(indicators);
        }
    }
}
