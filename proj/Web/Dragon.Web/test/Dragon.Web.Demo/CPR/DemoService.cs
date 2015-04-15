using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using Dragon.Data.Interfaces;
using Dragon.Web.Defaults;

namespace Dragon.Web.Demo.CPR
{
    public class DemoService : ServiceBase
    {
        // try with single commands first, then create autocommands derived from table

        [Import]
        public IRepository<DemoTable> DemoTables { get; set; }

        public ServiceResult<bool> Insert(string s)
        {
            return ExecuteCommand(new InsertCommandFor<DemoTable>()
            {
                Data = new DemoTable()
                {
                    DemoString = s
                }
            });
        }

        public ServiceResult<bool> Update(DemoUpdateCommand cmd)
        {
            return ExecuteCommand(cmd, () =>
            {
                Trace.WriteLine("Update");
                return new ServiceResult<bool>(true);
            });
        }

        public ServiceResult<bool> Delete(DemoDeleteCommand cmd)
        {
            return ExecuteCommand(cmd, () =>
            {
                Trace.WriteLine("Delete");
                return new ServiceResult<bool>(true);
            });
        }

        public IEnumerable<DemoTable> GetAll()
        {
            return DemoTables.GetAll();
        }


        public DemoTable Get(Guid id)
        {
            return DemoTables.Get(id);
        }
    }
}