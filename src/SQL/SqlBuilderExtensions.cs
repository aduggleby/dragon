using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dragon.SQL
{
    public static class SqlBuilderExtensions
    {
        public static Expr<T> IsEqual<T>(this Expr<T> baseExpr, Expression<Func<T,object>> propExpr, object value)
        {
            var newExpr = new IsEqual<T>(LambdaHelper.GetProperty<T>(propExpr), value);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> Like<T>(this Expr<T> baseExpr, Expression<Func<T, object>> propExpr, object value)
        {
            var newExpr = new Like<T>(LambdaHelper.GetProperty<T>(propExpr), value);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> GreaterThan<T>(this Expr<T> baseExpr, Expression<Func<T, object>> propExpr, object value)
        {
            var newExpr = new GreaterThan<T>(LambdaHelper.GetProperty<T>(propExpr), value);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> SmallerThan<T>(this Expr<T> baseExpr, Expression<Func<T, object>> propExpr, object value)
        {
            var newExpr = new SmallerThan<T>(LambdaHelper.GetProperty<T>(propExpr), value);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> GreaterThanOrEqualTo<T>(this Expr<T> baseExpr, Expression<Func<T, object>> propExpr, object value)
        {
            var newExpr = new GreaterThanOrEqualTo<T>(LambdaHelper.GetProperty<T>(propExpr), value);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> SmallerThanOrEqualTo<T>(this Expr<T> baseExpr, Expression<Func<T, object>> propExpr, object value)
        {
            var newExpr = new SmallerThanOrEqualTo<T>(LambdaHelper.GetProperty<T>(propExpr), value);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> And<T>(this Expr<T> baseExpr, Action<Expr<T>> expr)
        {
            var newExpr = new Dragon.SQL.And<T>(expr);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> Or<T>(this Expr<T> baseExpr, Action<Expr<T>> expr)
        {
            var newExpr = new Dragon.SQL.Or<T>(expr);
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> And<T>(this Expr<T> baseExpr)
        {
            var newExpr = new Dragon.SQL.And<T>();
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> Or<T>(this Expr<T> baseExpr)
        {
            var newExpr = new Dragon.SQL.Or<T>();
            baseExpr.Add(newExpr);
            return baseExpr;
        }

        public static Expr<T> Group<T>(this Expr<T> baseExpr, Action<Expr<T>> expr)
        {
            var newExpr = new Dragon.SQL.Group<T>(expr);
            baseExpr.Add(newExpr);
            return baseExpr;
        }
    }
}
