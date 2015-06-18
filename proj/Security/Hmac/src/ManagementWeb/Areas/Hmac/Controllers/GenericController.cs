using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web.Mvc;
using ManagementWeb.Areas.Hmac.Models;
using ManagementWeb.Areas.Hmac.Repositories;

namespace ManagementWeb.Areas.Hmac.Controllers
{
    public abstract class GenericController<TModel, TKey> : Controller where TModel : IModel<TKey>, new()
    {
        [Import]
        public IGenericRepository<TModel, TKey> Repository { get; set; }

        // GET: Model
        public async Task<ActionResult> Index()
        {
            try
            {
                return View("List", await Repository.List());
            }
            catch (Exception e)
            {
                SetError(e);
                return View("List", new List<TModel>());
            }
        }

        // GET: Model/Details/5
        public async Task<ActionResult> Details(TKey id)
        {
            try
            {
                return View(await Repository.Details(id));
            }
            catch (Exception e)
            {
                SetError(e);
                return View(new TModel());
            }
        }

        // GET: Model/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Model/Create
        [HttpPost]
        public async Task<ActionResult> Create(TModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await Repository.Add(model);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                SetError(e);
                return View(model);
            }
        }

        // GET: Model/Edit/5
        public async Task<ActionResult> Edit(TKey id)
        {
            try
            {
                return View(await Repository.Details(id));
            }
            catch (Exception e)
            {
                SetError(e);
                return View(new TModel());
            }
        }

        // POST: Model/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(TKey id, TModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                model.Id = id;
                await Repository.Edit(model);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                SetError(e);
                return View(model);
            }
        }

        // GET: Model/Delete/5
        public async Task<ActionResult> Delete(TKey id)
        {
            try
            {
                return View(await Repository.Details(id));
            }
            catch (Exception e)
            {
                SetError(e);
                return View(new TModel());
            }
        }

        // POST: Model/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(TKey id, TModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await Repository.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                SetError(e);
                return View(model);
            }
        }

        private void SetError(Exception e)
        {
            ViewBag.ErrorMessage = e.Message;
        }
    }
}
