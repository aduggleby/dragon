using System;
using Dragon.CPR.Attributes;
using Dragon.Data.Attributes;

namespace Dragon.CPR
{
    public class Command
    {
        [Key]
        public Guid CommandID { get; set; }

        [Length("200")]
        public string Type { get; set; }

        [Length("MAX")]
        public string JSON { get; set; }

        public Guid UserID { get; set; }

        public DateTime Executed { get; set; }
    }
}
