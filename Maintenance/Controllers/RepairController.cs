using Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Project.Domain;
using Project.ViewModels;

namespace Project.Controllers
{
    public class RepairController : Controller
    {
        RepairClass repairClass = new RepairClass();
        RepairEntityClass repairEntity = new RepairEntityClass();
        String path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\";

        [HttpGet]
        public object GetExistedRepair(string Date, string FactoryNumber)
        {
            return repairEntity.IsExistedRepair(FactoryNumber, Date);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(repairClass);
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            var newRepair = repairEntity.Create(form);
            Session["LastUpdatedOrCreatedItem"] = Convert.ToInt32(newRepair["LastRepairID"].ToString());
            return File(new FileStream(path + newRepair["File"].ToString(), FileMode.Open), "application/docx",
                newRepair["File"].ToString());
        }
       
        [HttpGet]
        public ActionResult Delete(int Id, bool Status)
        {
            ViewBag.ID = Id;
            return View(repairClass);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            repairEntity.Delete(id);
            return RedirectToAction("Repair", "Main");
        }

        [HttpGet]
        public ActionResult Restore(int Id, bool Status)
        {
            ViewBag.ID = Id;
            return View(repairClass);
        }

        [HttpPost]
        public ActionResult Restore(int id)
        {
            repairEntity.Restore(id);
            return RedirectToAction("DeletedRepairs", "Main");
        }

        [HttpGet]
        public ActionResult Update(int Id, string Status, string ProductID)
        {
            ViewBag.ID = Id;
            ViewBag.Status = Status;
            ViewBag.ProductID = ProductID;
            return View(repairClass);
        }

        [HttpPost]
        public ActionResult Update(int id, FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            repairEntity.Update(id, form);
            Session["LastUpdatedOrCreatedItem"] = id;
            return RedirectToAction("Repair", "Main");
        }

        [HttpGet]
        public ActionResult Return(int id, string Status)
        {
            ViewBag.ID = id;
            ViewBag.Status = Status;
            var repair = repairEntity.GetRepair(id);
            return View(repair);
        }

        [HttpPost]
        public ActionResult Return(int id, FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            repairEntity.Return(id, form);
            Session["LastUpdatedOrCreatedItem"] = id;
            return RedirectToAction("Repair", "Main");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var repair = repairEntity.GetRepair(id);
            return View(repair);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            repairEntity.Edit(id, form);
            Session["LastUpdatedOrCreatedItem"] = id;
            return RedirectToAction("Repair", "Main");
        }

        [HttpGet]
        public ActionResult EditJobs(int id)
        {
            var repair = repairEntity.GetRepair(id);
            ViewBag.Materials = repair.Repairs.Materials != null ? repair.Repairs.Materials : "";
            ViewBag.Jobs = repair.Repairs.Jobs;
            ViewBag.RepairerId = repair.Repairs.ResponsiblePersonId != 0 ? repair.Repairs.ResponsiblePersonId : 0;
            return View(repair);
        }

        [HttpPost]
        public ActionResult EditJobs(int id, FormCollection collection)
        {
            Dictionary<string, string> form = collection.AllKeys.ToDictionary(k => k, v => collection[v]);
            repairEntity.EditJobs(id, form);
            Session["LastUpdatedOrCreatedItem"] = id;
            return RedirectToAction("Repair", "Main", new { condition = Session["Condition"], 
                    key = Session["Key"], dateFrom = Session["dateFrom"], toDate = Session["toDate"] });
        }

        [HttpGet]
        public ActionResult Acct(string ids)
        {
            var clientRepair = repairEntity.CreateAcct(ids);
            var responsibles = repairEntity.dataBase.ResponsiblePersons;
            ViewBag.Client = clientRepair.Client;
            ViewBag.ClientLastContractDate = clientRepair.ClientLastContractDate;
            ViewBag.ClientLastContractNumber = clientRepair.ClientContractNumber;
            ViewBag.Value = clientRepair.ValueForJob;
            ViewBag.IDs = clientRepair.Ids;
            ViewBag.RepairAndMaterials = clientRepair.RepairAndMaterials;
            ViewBag.ResponsiblePersons = responsibles;
            ViewBag.TResponsiblePersons = clientRepair.TResponsible;
            return View();
        }

        [HttpPost]
        public FileResult Acct(string id, FormCollection collection)
        {
            int idx = 0;
            List<NormsOfTime> normsOfTime = new List<NormsOfTime>();
            List<int> repairIds = new List<int>();
            Session["LastUpdatedOrCreatedItem"] = Convert.ToInt32(id.Split(',').FirstOrDefault());
            foreach (var ids in id.Split(','))
            {
                normsOfTime.Add(new NormsOfTime() 
                {
                   InputControl = double.Parse(collection["InputControl-" + idx], System.Globalization.CultureInfo.InvariantCulture),
                   SearchDefects = double.Parse(collection["SearchDefects-" + idx], System.Globalization.CultureInfo.InvariantCulture),
                   RestoreDefects = double.Parse(collection["RestoreDefects-" + idx], System.Globalization.CultureInfo.InvariantCulture),
                   UpdateOS = double.Parse(collection["UpdateOS-" + idx], System.Globalization.CultureInfo.InvariantCulture),
                   Check = double.Parse(collection["Check-" + idx], System.Globalization.CultureInfo.InvariantCulture),
                });
                repairIds.Add(Convert.ToInt32(ids));
                idx++;
            }
            ResponsiblePerson OurResponsible = new ResponsiblePerson();
            OurResponsible.shortName = collection["ORPShort"].ToString();
            OurResponsible.name = collection["ORPLong"].ToString();
            OurResponsible.JobTitle = collection["ORPTitle"].ToString();
            OurResponsible.Base = collection["ContractBase"].ToString();

            ResponsiblePerson ThierResponsible = new ResponsiblePerson();
            ThierResponsible.shortName = collection["CRPShort"].ToString();
            ThierResponsible.name = collection["CRPLong"].ToString();
            ThierResponsible.JobTitle = collection["CRPTitle"].ToString();

            var actNumberInDoc = collection["ActNumberInDoc"].ToString() != "" ? collection["ActNumberInDoc"].ToString() : "";
            var dateToAct = collection["DateToAcct"].ToString() != "" ? collection["DateToAcct"].ToString() : "";
            var actNumber = collection["AcctNumber"].ToString() != "" ? collection["AcctNumber"].ToString() : "";
            //var lastContractNumber = collection["LastContractNumber"].ToString() != "" ? collection["LastContractNumber"].ToString() : "";
            //var lastContractDate = collection["LastContractDate"].ToString() != "" ? collection["LastContractDate"].ToString() : "";

            var exportFileName = repairEntity.AcctToExcel(OurResponsible, ThierResponsible, repairIds, normsOfTime,actNumberInDoc,
                dateToAct, collection["LastContractNumber"], collection["LastContractDate"], actNumber);
            var path = AppDomain.CurrentDomain.BaseDirectory + "App_Data/";
            string exportFilePath = path + exportFileName;
            return File(new FileStream(exportFilePath, FileMode.Open), "application/xls", 
                "Счёт " + collection["AcctNumber"].ToString() + " " + collection["Client"] + " "
                + collection["DateToAcct"].ToString() + ".xls");
        }
    }
}