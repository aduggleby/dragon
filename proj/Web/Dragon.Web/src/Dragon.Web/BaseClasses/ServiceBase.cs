using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
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
        public IContext Context { get; set; }

        [Import]
        public Container Container { get; set; }

        [Import]
        public ICommandPersister CommandPersister { get; set; }


        protected ServiceBase()
        {
        }

        public ServiceResult<bool> ExecuteCommand<T>(T command)
            where T : CommandBase
        {
            // if execution was not halted, then consider a success
            return ExecuteCommand(command, () => new ServiceResult<bool>(!command.HaltExecution));
        }

        public ServiceResult<TR> ExecuteCommand<TR, T>(T command, Func<ServiceResult<TR>> processor)
            where T : CommandBase
        {
            using (var tx = new TransactionScope())
            {
                ServiceResult<TR> sr = null;
                try
                {
                    Trace.WriteLine(string.Format("Executing command {0}", typeof(T)));
                    if (command.ExecutionAllowed(Context))
                    {
                        //
                        // Process command
                        //
                        if (!command.HaltExecution)
                            Container.GetAllInstances<ICommandBeforeProcess<T>>()
                                .ToList()
                                .ForEach(x =>
                                {
                                    Trace.Write(string.Format("Before Processor {0} ... ", x));
                                    x.BeforeProcess(command);
                                    Trace.WriteLine(string.Format(" Halted: {0}", command.HaltExecution));
                                });

                        if (!command.HaltExecution) sr = processor();

                        if (!command.HaltExecution)
                            Container.GetAllInstances<ICommandBeforeSave<T>>()
                                .ToList()
                                .ForEach(x =>
                                {
                                    Trace.Write(string.Format("Before Save {0} ... ", x));
                                    x.BeforeSave(command);
                                    Trace.WriteLine(string.Format(" Halted: {0}", command.HaltExecution));
                                });

                        //
                        // Persist command to command log
                        //
                        if (!command.HaltExecution)
                        {
                            Trace.WriteLine(string.Format("Persisting command..."));
                            CommandPersister.Persist(command);
                        }

                        //
                        // Run the default project = saver
                        //
                        if (!command.HaltExecution)
                        {
                            var saver = Container.GetInstance<ICommandSave<T>>();

                            Trace.Write(string.Format("Command Saver / Default Projection {0} ... ", saver));
                            saver.Save(command);
                            Trace.WriteLine(string.Format(" Halted: {0}", command.HaltExecution));

                        }

                        // 
                        // Run additional projections
                        //
                        if (!command.HaltExecution)
                            Container.GetAllInstances<ICommandAfterSave<T>>()
                                .ToList()
                                .ForEach(x =>
                                     {
                                         Trace.Write(string.Format("After Save {0} ... ", x));
                                         x.AfterSave(command);
                                         Trace.WriteLine(string.Format(" Halted: {0}", command.HaltExecution));
                                     });

                        if (!command.HaltExecution) tx.Complete();
                        Trace.WriteLine(string.Format("Executed command {0} - Execution halted: {1}", typeof(T), command.HaltExecution));


                    }
                    else
                    {
                        Trace.WriteLine(string.Format("Cannot execute command {0} - Execution not allowed.", typeof(T)));
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(string.Format("Exception executing command {0}:\r\n{1}", typeof(T), ex.ToString()));

                    throw;
                }
                return sr;
            }
        }
    }
}
