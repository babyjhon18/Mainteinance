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
    public class JobsController : Controller
    {
        JobEntityClass jobEntity = new JobEntityClass();
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(string JobName, string JobTime)
        {
            jobEntity.Create(JobName, JobTime);
            return RedirectToAction("Jobs", "Main");
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            var jobs = jobEntity.GetJobs(id);
            return View(jobs);
        }
        [HttpPost]
        public ActionResult Update(Jobs jobs)
        {
            jobEntity.Update(jobs);
            return RedirectToAction("Jobs", "Main");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var jobs = jobEntity.GetJobs(id);
            return View(jobs); ;
        }
        [HttpPost]
        public ActionResult Delete(Jobs jobs)
        {
            jobEntity.Delete(jobs);
            return RedirectToAction("Jobs", "Main");
        }
    }
}