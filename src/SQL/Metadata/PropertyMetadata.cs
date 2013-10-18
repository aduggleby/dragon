using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dragon.SQL
{
    [Serializable]
    public class PropertyMetadata
    {
        public bool IsPK { get; set; }
        public bool Indexed { get; set; }
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public string Length { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string SqlTypeString { get; set; }
        public string SqlKeyTypeString { get; set; }
        public bool Nullable { get; set; }

    }
}
