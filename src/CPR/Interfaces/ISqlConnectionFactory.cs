using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Dragon.CPR.Interfaces
{
    public interface ISqlConnectionFactory
    {
        SqlConnection New(Type asking);
    }
}
