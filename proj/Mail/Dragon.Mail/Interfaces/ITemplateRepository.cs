using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Mail.Models;

namespace Dragon.Mail.Interfaces
{
    public interface ITemplateRepository
    {
        void EnumerateTemplates(Action<Template> act);
    }
}
