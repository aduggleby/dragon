using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Data.Interfaces;
using Dragon.Web.Interfaces;

namespace Dragon.Web.Defaults
{
   public class DefaultCommandPersister : ICommandPersister
   {
       private IContext m_context;
       private ICommandSerializer m_serializer;
       private IRepository<CommandTable> m_repository;

       public DefaultCommandPersister(
           ICommandSerializer serializer,
           IContext context,
           IRepository<CommandTable> repository)
       {
           m_serializer = serializer;
           m_context = context;
           m_repository = repository;
       }

       public void Persist(object command)
       {
           var json = m_serializer.Serialize(command);

           var persistedCmd = new CommandTable()
           {
               Object = json,
               Type = command.GetType().FullName
           };

           if (m_context != null)
           {
               persistedCmd.UserID = m_context.UserID;
           }

           m_repository.Insert(persistedCmd);
       }
    }
}
