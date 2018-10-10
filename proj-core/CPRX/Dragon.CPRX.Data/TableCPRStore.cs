using Dragon.Data.Interfaces;
using Dragon.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dragon.CPRX.Data
{
    public class TableCPRStore : ICPRStore
    {
        private IRepository<CommandTable> m_commandRepository;

        public TableCPRStore(IRepository<CommandTable> commandRepository, IConfiguration config, ILoggerFactory loggerFactory)
        {
            m_commandRepository = commandRepository;

            var s = new RepositorySetup(new DefaultDbConnectionContextFactory(config, loggerFactory), config, loggerFactory);
            s.EnsureTableExists<CommandTable>();
        }

        public void Persist(CPRCommand cmd, Guid executingUserID, DateTime utcBeganProjections)
        {
            var cmdRec = new CommandTable();
            cmdRec.CommandID = cmd.ID;
            cmdRec.Executed = utcBeganProjections;
            cmdRec.UserID = executingUserID;
            cmdRec.Type = cmd.GetType().ToString();
            cmdRec.JSON = JsonConvert.SerializeObject(cmd, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });

            m_commandRepository.Insert(cmdRec);
        }
    }
}
