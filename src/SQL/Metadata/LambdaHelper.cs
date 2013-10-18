using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dragon.SQL
{
    public class LambdaHelper
    {
        public static string GetProperty(Expression<Func<object, object>> exp)
        {
           return GetProperty<object>(exp);
        }

        public static string GetProperty<T>(Expression<Func<T,object>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            var name = body.Member.Name;
            return name;
        }
    }
}
