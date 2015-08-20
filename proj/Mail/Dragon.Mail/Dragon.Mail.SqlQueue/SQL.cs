using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.SqlQueue
{
    public class SQL
    {
        public const string GET_EARLIEST =
            @"  SELECT * 
                FROM   {TABLE} 
                WHERE  Sent = 0 
                AND    UserID IN 
                    (   SELECT TOP 1 UserID 
                        FROM {TABLE} 
                        WHERE Processed = 0 
                        ORDER BY NextProcessingAttemptUTC
                    )";

        public const string GET_LASTSENT_FORUSER =
                @"SELECT TOP 1 * FROM {TABLE} WHERE SENT = 1 WHERE UserID = @UserID ORDER BY SentUTC DESC ";
    
    }
}
