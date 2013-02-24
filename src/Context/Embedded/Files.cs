using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Common.Util;

namespace Dragon.Context.Embedded
{
    public class Files
    {
        public static byte[] FacebookJS()
        {
            var filename = string.Concat(typeof(DragonContext).Namespace, ".Embedded.facebook.js");
            return EmbeddedUtil.GetFileContents<Files>(filename);
        }
    }
}
