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
    public abstract class CreateUpdateTableControllerBase<TCreate, TUpdate, TTable> : TableControllerBase<TTable>
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

        public CreateUpdateTableControllerBase():base()
        {

        }

        public override ActionResult Index(Guid? id)
        {
            AddViewLink(Url.Action("Create"), "Create");
            return base.Index(id);
        }
        
        [HttpGet]
        public virtual ActionResult Create(TCreate cmd)
        {
            ViewData["button-continue"] = "Create";
            AddCustomLessCss();
            PreCreate(cmd);
            return CommandView(cmd);
        }
        
        [HttpPost]
        public virtual ActionResult Create(TCreate cmd, FormCollection fc)
        {
            ViewData["button-continue"] = "Create";
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
            ViewData["button-continue"] = "Save";

            cmd = cmd.CommandID == Guid.Empty ? Load(id) : cmd;
            PreUpdate(id, cmd);
            if (cmd == null) return new HttpNotFoundResult();
            return UpdateView(cmd);
        }

        protected virtual ActionResult UpdateView(TUpdate cmd)
        {
            return CommandView(cmd);
        }

        [HttpPost]
        public virtual ActionResult Update(Guid id, TUpdate cmd, FormCollection fc)
        {
            ViewData["button-continue"] = "Save";

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
            var tableData = Repository.Get(id);
            if (tableData == null) return default(TUpdate);
            Mapper.Map(tableData, cmd);
            return cmd;
        }
        
        protected virtual ActionResult AfterProcess(
            TCreate cmd,
            Func<object, ActionResult> success,
            Func<object, ActionResult> error)
        {
            return AfterProcess(cmd, CreateCommandDispatcher, success, error);
        }

        protected virtual ActionResult AfterProcess(
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