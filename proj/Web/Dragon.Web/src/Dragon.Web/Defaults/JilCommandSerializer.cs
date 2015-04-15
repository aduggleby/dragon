using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dragon.Web.Interfaces;
using Jil;

namespace Dragon.Web.Defaults
{
    public class JilCommandSerializer : ICommandSerializer
    {
        private readonly Options m_jilOptions;

        public JilCommandSerializer()
        {
            m_jilOptions = new Options(
                     includeInherited: true,
                     dateFormat: DateTimeFormat.ISO8601,
                     unspecifiedDateTimeKindBehavior: UnspecifiedDateTimeKindBehavior.IsUTC);
        }

        public string Serialize(object command)
        {
            return JSON.Serialize(command, m_jilOptions);
        }
    }
}
