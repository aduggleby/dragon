using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dragon.Data.Repositories;
using Dragon.Web.Defaults;
using Dragon.Web.Demo.CPR;
using Dragon.Web.Utils;

namespace Dragon.Web.Demo.Controllers
{
    public class HomeController : Controller
    {
        private PersistableSetup m_setup;

        [Import]
        public DemoService DemoService { get; set; }
        
        public HomeController()
        {
            m_setup = new PersistableSetup(new RepositorySetup());
            m_setup.EnsureTable<CommandTable>();
            m_setup.EnsureTable<DemoTable>();
        }

        public ActionResult Index()
        {
            var model = DemoService.GetAll();
            return View(model);
        }

        [HttpGet]
        public ActionResult Insert()
        {
            var model = new DemoInsertCommand();
            return View(model);
        }

        [HttpPost]
        public ActionResult Insert(DemoInsertCommand cmd)
        {
            if (DemoService.Insert(cmd).Payload)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(cmd);
            }
        }

        [HttpGet]
        public ActionResult Details(Guid id)
        {
            var model = DemoService.Get(id);
            return View(model);
        }


        [HttpGet]
        public ActionResult Update(Guid id)
        {
            var record = DemoService.Get(id);
            var model= new DemoUpdateCommand();
            model.DemoString = record.DemoString;
            return View(model);
        }

        [HttpPost]
        public ActionResult Update(Guid id, DemoUpdateCommand cmd)
        {
            cmd.ID = id;
            if (DemoService.Update(cmd).Payload)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(cmd);
            }
        }

        [HttpGet]
        public ActionResult Delete(Guid id)
        {
            var model = DemoService.Get(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(Guid id, FormCollection fc)
        {
            if (DemoService.Delete(new DemoDeleteCommand(){ ID = id }).Payload)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Delete");
            }
        }
    }
}