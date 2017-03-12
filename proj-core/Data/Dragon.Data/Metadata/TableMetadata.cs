using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Data
{
    public class TableMetadata
    {
        public string ClassName { get; set; }

        public string TableName { get; set; }
        public string Schema { get; set; }

        public List<PropertyMetadata> Properties { get; set; }

        public TableMetadata()
        {
            Properties = new List<PropertyMetadata>();
        }
    }
}
