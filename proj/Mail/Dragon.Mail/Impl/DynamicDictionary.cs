using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.Mail.Impl
{
    public class DynamicDictionary : DynamicObject
    {
        Dictionary<string, object> dict;

        public DynamicDictionary(Dictionary<string, object> dict)
        {
            this.dict = dict;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dict[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dict.TryGetValue(binder.Name, out result);
        }
    }
}
