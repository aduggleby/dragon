using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dragon.Data.Interfaces;
using Dragon.Web.Defaults;
using Jil;
using Dragon.Web.Interfaces;
using SimpleInjector;


namespace Dragon.Web
{
    public abstract class ServiceBase
    {
        [Import]
        public IRepository<CommandTable> CommandRep { get; set; }

        [Import]
        public IContext Context { get; set; }

        [Import]
        public Container Container { get; set; }

        private readonly Options m_jilOptions;

        protected ServiceBase()
        {
            m_jilOptions = new Options(
                      dateFormat: DateTimeFormat.ISO8601,
                      unspecifiedDateTimeKindBehavior: UnspecifiedDateTimeKindBehavior.IsUTC);
        }

        public ServiceResult<TR> ExecuteCommand<TR, T>(T command, Func<ServiceResult<TR>> processor)
            where T : CommandBase
        {
            using (var tx = new TransactionScope())
            {
                ServiceResult<TR> sr = null;
                try
                {
                    Container.GetAllInstances<ICommandBeforeProcess<T>>().ToList().ForEach(x => x.BeforeProcess(command));

                    sr = processor();

                    Container.GetAllInstances<ICommandBeforeSave<T>>().ToList().ForEach(x => x.BeforeSave(command));

                    var json = JSON.Serialize(command, m_jilOptions);

                    var persistedCmd = new CommandTable()
                    {
                        Object = json,
                        Type = command.GetType().FullName
                    };

                    if (Context != null)
                    {
                        persistedCmd.UserID = Context.UserID;
                    }

                    CommandRep.Insert(persistedCmd);

                    var saver = Container.GetInstance<ICommandSave<T>>();
                    saver.Save(command);

                    Container.GetAllInstances<ICommandAfterSave<T>>().ToList().ForEach(x => x.AfterSave(command));

                    tx.Complete();
                }
                catch (Exception ex)
                {
                    throw;
                }
                return sr;
            }
        }
    }
}
