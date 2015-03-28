using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Web.Interfaces;
using SimpleInjector;

namespace Dragon.Web
{
    public class TableBaseProcessor : ITableBeforeSave<TableBase>
    {
        private IContext m_context;

        public TableBaseProcessor(IContext context)
        {
            m_context = context;
        }

        public void BeforeSave(TableBase obj)
        {
            obj.Modified = DateTime.UtcNow;
            obj.ModifiedBy = m_context.UserID;

            if (obj.Created == DateTime.MinValue)
            {
                obj.Created = DateTime.UtcNow;
                obj.CreatedBy = m_context.UserID;
            }

            obj.ContextID = m_context.ContextID;
        }
    }
}
