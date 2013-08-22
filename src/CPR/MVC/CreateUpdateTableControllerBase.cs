using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Dragon.CPR;
using Dragon.CPR.Sql.Grid;
using Dragon.Interfaces;

namespace Dragon.CPR.MVC
{
    public abstract class CreateUpdateTableControllerBase<TCreate, TUpdate, TTable> : CPRControllerBase
        where TCreate : CommandBase
        where TUpdate : CommandBase
        where TTable : class
    {
        static CreateUpdateTableControllerBase()
        {
            Mapper.CreateMap<TTable, TUpdate>();
        }

        public CommandDispatcher<TCreate> CreateCommandDispatcher { get; set; }
        public CommandDispatcher<TUpdate> UpdateCommandDispatcher { get; set; }

        public CreateUpdateTableControllerBase(IPermissionStore permissionStore)
        {

        }

        public virtual ActionResult Index(Guid? id)
        {
            PreIndex();
            AddViewLink(Url.Action("Create"), "Create");
            return View();
        }

        public virtual ActionResult FXGrid(SortingPagingViewModel sortingPaging, string filterString)
        {
            var model = new TableViewModel<TTable>(sortingPaging, "Created", false);

            BuildIndex(model);
           
            ReadModelRepository.PopulateTable(model);

            return PartialView("_FXGrid", model);

        }
        

        [HttpGet]
        public virtual ActionResult Create(TCreate cmd)
        {
            AddCustomLessCss();
            PreCreate(cmd);
            return CommandView(cmd);
        }
        
        [HttpPost]
        public virtual ActionResult Create(TCreate cmd, FormCollection fc)
        {
            PreCreate(cmd);
            return AfterProcess(cmd,
                    (success) =>
                    {
                        PostCreate(cmd);
                        return RedirectAfterCreate(cmd);
                    },
                    (error) =>
                    {
                        return Create(cmd);
                    });
        }
        
        [HttpGet]
        public virtual ActionResult Update(Guid id, TUpdate cmd = null)
        {
            cmd = cmd.CommandID == Guid.Empty ? Load(id) : cmd;
            PreUpdate(id, cmd);
            if (cmd == null) return new HttpNotFoundResult();
            return CommandView(cmd);
        }

        [HttpPost]
        public virtual ActionResult Update(Guid id, TUpdate cmd, FormCollection fc)
        {
            PreUpdate(id, cmd);
            return AfterProcess(cmd,
                    (success) =>
                    {
                        PostUpdate(id, cmd);
                        return RedirectAfterUpdate(cmd);
                    },
                    (error) =>
                    {
                        return Update(id, cmd);
                    });
        }

        protected virtual ActionResult RedirectAfterCreate(TCreate cmd)
        {
            return RedirectToAction("Index");            
        }

        protected virtual ActionResult RedirectAfterUpdate(TUpdate cmd)
        {
            return RedirectToAction("Index");
        }

        protected virtual void PreIndex()
        {
        }

        protected virtual void BuildIndex(TableViewModel<TTable> model)
        {
        }

        protected virtual void PreCreate(TCreate cmd)
        {
        }

        protected virtual void PreUpdate(Guid id, TUpdate cmd)
        {
        }

        protected virtual void PostCreate(TCreate cmd)
        {
        }

        protected virtual void PostUpdate(Guid id, TUpdate cmd)
        {
        }
        
        protected TUpdate Load(Guid id)
        {
            var cmd = Activator.CreateInstance<TUpdate>();
            var tableData = ReadModelRepository.Get<TTable>(id);
            if (tableData == null) return default(TUpdate);
            Mapper.Map(tableData, cmd);
            return cmd;
        }
        
        protected ActionResult AfterProcess(
            TCreate cmd,
            Func<object, ActionResult> success,
            Func<object, ActionResult> error)
        {
            return AfterProcess(cmd, CreateCommandDispatcher, success, error);
        }

        protected ActionResult AfterProcess(
            TUpdate cmd,
            Func<object, ActionResult> success,
            Func<object, ActionResult> error)
        {
            return AfterProcess(cmd, UpdateCommandDispatcher, success, error);
        }

        private ActionResult AfterProcess<T>(
            T cmd,
            CommandDispatcher<T> dispatcher,
            Func<object, ActionResult> success,
            Func<object, ActionResult> error)
            where T : CommandBase
        {
            var err = dispatcher.Dispatch(cmd);
            if (!err.Any())
            {
                return success(null);
            }
            else
            {
                foreach (var e in err)
                {
                    ModelState.AddModelError(e.PropertyName ?? String.Empty, e.Message);
                }
                return error(null);
            }
        }
    }
}