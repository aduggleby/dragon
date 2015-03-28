using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using Dragon.Data.Interfaces;
using Dragon.Web.Interfaces;

namespace Dragon.Web.Demo.CPR
{
    public class DemoDeleteCommandProcessor : ICommandSave<DemoDeleteCommand>
    {
        [Import]
        public IRepository<DemoTable> DemoTables { get; set; } 

        public void Save(DemoDeleteCommand obj)
        {
            DemoTables.Delete(DemoTables.Get(obj.ID));
        }
    }
}