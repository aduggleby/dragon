using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;
using Dragon.CPR.Errors;
using Dragon.CPR.Impl.Projections;
using Dragon.CPR.Interfaces;
using Dragon.Data.Interfaces;
using Dragon.Context;
using Newtonsoft.Json;
using StructureMap;

namespace Dragon.CPR
{
    public class CommandDispatcher<TCommand> : DispatcherBase<TCommand>
        where TCommand : CommandBase
    {
        private readonly IProjection<TCommand>[] m_projections;
        private readonly IProjection<object>[] m_genericprojections;

        private readonly IHandler<TCommand>[] m_handlers;
        private readonly IRepository<Command> m_commandRepository;
        private readonly JsonSerializerSettings m_jsonSerializerSettings;

        public CommandDispatcher(IRepository<Command> r, IContainer container)
        {
            m_jsonSerializerSettings = new JsonSerializerSettings();

            m_jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            m_jsonSerializerSettings.TypeNameHandling = TypeNameHandling.All;

            m_projections = container.GetAllInstances<IProjection<TCommand>>().ToArray();
            m_genericprojections = container.GetAllInstances<IProjection<object>>().ToArray();

            m_handlers = container.GetAllInstances<IHandler<TCommand>>().OrderBy(x => x.Order).ToArray();

            m_commandRepository = r;
        }

        public override IEnumerable<ErrorBase> Dispatch(TCommand o)
        {
            using (var tx = new TransactionScope())
            {
                if (o.CommandID == Guid.Empty) o.CommandID = Guid.NewGuid();

                var errors = Handle(o).ToList();
                if (!errors.Any())
                {
                    Persist(o);
                    Project(o);
                }
                
                tx.Complete();

                return errors;
            }
        }

        public IEnumerable<ErrorBase> Handle(TCommand o)
        {
            var l = new List<ErrorBase>();
            foreach (var handlerGroup in m_handlers.GroupBy(x => x.Order).OrderBy(X => X.Key))
            {
                foreach (var handler in handlerGroup)
                {
                    l = l.Union(handler.Handle(o).ToList()).ToList();
                }

                // break if a specific level of handlers has an error, 
                // so that we get security first, validation second and database stuff third 
                // and don't process higher level stages
                if (l.Any()) return l;
            }
            return l;
        }

        public void Persist(TCommand o)
        {
            var cmd = new Command();
            cmd.CommandID = o.CommandID;
            cmd.Executed = DateTime.UtcNow;
            cmd.UserID = o.ExecutingUserID;
            cmd.Type = o.GetType().ToString();
            cmd.JSON = JsonConvert.SerializeObject(o, m_jsonSerializerSettings);

            m_commandRepository.Insert(cmd);
        }

        public void Project(TCommand o)
        {
            foreach (var projection in m_projections)
            {
                var usesCommand = projection as IUseCommand<TCommand>;
                if (usesCommand != null)
                {
                    usesCommand.Command = o;
                }
                
                projection.Project(o);
            }

            foreach (var projection in m_genericprojections)
            {
                projection.Project(o);
            }
        }
    }
}
