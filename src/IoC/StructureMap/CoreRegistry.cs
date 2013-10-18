using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.ActivityCenter;
using Dragon.Core.Mail;
using Dragon.CPR;
using Dragon.CPR.Attributes;
using Dragon.CPR.Impl.Projections;
using Dragon.CPR.Interfaces;
using Dragon.Interfaces;
using Dragon.Interfaces.ActivityCenter;
using Dragon.Interfaces.Core;
using Dragon.Notification;
using StructureMap.Graph;

namespace Dragon.IoC.StructureMap
{
    public class CoreRegistry : RegistryBase
    {
        public CoreRegistry()
        {
            For<IEmailService>().Use<NetEmailService>();
            For<IEmailTemplateService>().Use<Antlr4StringTemplateService>();
        }
    }
}
