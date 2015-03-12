using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Context.Interfaces;
using Dragon.Data.Attributes;

namespace Dragon.Context.Users
{
    public class DragonRegistration : IRegistration, IDragonTable
    {
        [Key]
        public Guid RegistrationID { get; set; }
        public Guid UserID { get; set; }

        [Length(200)]
        public string Service { get; set; }
        [Length(200)]
        public string Key { get; set; }
        [Length(200)]
        public string Secret { get; set; }
    }
}
