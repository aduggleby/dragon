using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dragon.SQL
{
    [DebuggerDisplay("{NameAndCount}")]
    public abstract class Expr<T> : List<Expr<T>>
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, TableMetadata> s_metadata =
         new ConcurrentDictionary<RuntimeTypeHandle, TableMetadata>();

        protected static TableMetadata Metadata()
        {
            Type type = typeof(T);
            TableMetadata metadata;
            if (s_metadata.TryGetValue(type.TypeHandle, out metadata))
            {
                return metadata;
            }

            metadata = new TableMetadata();
            MetadataHelper.MetadataForClass(type, ref metadata);
            s_metadata[type.TypeHandle] = metadata;
            return metadata;
        }

        public virtual StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            foreach (var expr in this)
            {
                expr.Build(param, sb);
            }
            return sb;
        }

        public string NameAndCount
        {
            get
            {
                return this.GetType().Name + " (" + this.Count() + ")";
            }
        }
    }

    public abstract class AtomicExpr<T> : Expr<T>
    {

    }

    public abstract class GroupExpr<T> : Expr<T>
    {
        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            if (this.Any())
            {
                sb.Append("(");
                foreach (var expr in this)
                {
                    expr.Build(param, sb);
                }
                sb.Append(")");
            }
            return sb;
        }

    }


    public class Empty<T> : AtomicExpr<T>
    {
        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            return sb ?? new StringBuilder();
        }
    }

    public class True<T> : AtomicExpr<T>
    {
        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            sb.Append("1=1");
            return sb;
        }
    }

    public class Where<T> : Expr<T>
        where T : class
    {
        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            sb.Append("WHERE ");
            return base.Build(param, sb);
        }
    }

    public class Group<T> : GroupExpr<T>
    {
        protected Action<Expr<T>> m_actionToBuildGroup;

        public Group()
        {
            m_actionToBuildGroup = null;
        }

        public Group(Action<Expr<T>> inner)
        {
            m_actionToBuildGroup = inner;
        }


        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();

            if (m_actionToBuildGroup != null)
            {
                m_actionToBuildGroup(this);
            }
            
            if (this.Any())
            {
                sb.Append("(");
                foreach (var expr in this)
                {
                    expr.Build(param, sb);
                }
                sb.Append(")");
            }

            return sb;
        }
    }

    public class And<T> : Group<T>
    {
        public And()
        {
        }

        public And(Action<Expr<T>> inner):base(inner)
        {
         
        }

        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            sb.Append(" AND ");
            return base.Build(param, sb);
        }
    }

    public class Or<T> : Group<T>
    {
        public Or()
        {
        }

        public Or(Action<Expr<T>> inner)
            : base(inner)
        {

        }

        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            sb = sb ?? new StringBuilder();
            sb.Append(" OR ");
            return base.Build(param, sb);
        }
    }

    public abstract class ComparisonExpr<T> : Expr<T>
    {
        private string m_key;
        private object m_value;

        protected ComparisonExpr(string key, object value)
        {
            m_key = key;
            m_value = value;
        }

        protected abstract string ComparisonQualifier { get; }

        public override StringBuilder Build(Dictionary<string, object> param, StringBuilder sb = null)
        {
            var column = Expr<T>.Metadata().Properties.First(x => x.PropertyName == m_key);
            
            var p = TSQLGenerator.FindUniqueNameInDictionary(m_key, param);
            param.Add(p, m_value);

            sb = sb ?? new StringBuilder();
            sb.AppendFormat("[{0}]{2}@{1}", column.ColumnName, p, ComparisonQualifier);
            return sb;
        }
    }

    public class IsEqual<T> : ComparisonExpr<T>
    {
        public IsEqual(string key, object value)
            : base(key, value)
        {

        }

        protected override string ComparisonQualifier
        {
            get { return "="; }
        }
    }

    public class SmallerThan<T> : ComparisonExpr<T>
    {
        public SmallerThan(string key, object value)
            : base(key, value)
        {

        }

        protected override string ComparisonQualifier
        {
            get { return "<"; }
        }
    }

    public class GreaterThan<T> : ComparisonExpr<T>
    {
        public GreaterThan(string key, object value)
            : base(key, value)
        {

        }

        protected override string ComparisonQualifier
        {
            get { return ">"; }
        }
    }

    public class SmallerThanOrEqualTo<T> : ComparisonExpr<T>
    {
        public SmallerThanOrEqualTo(string key, object value)
            : base(key, value)
        {

        }

        protected override string ComparisonQualifier
        {
            get { return "<="; }
        }
    }

    public class GreaterThanOrEqualTo<T> : ComparisonExpr<T>
    {
        public GreaterThanOrEqualTo(string key, object value)
            : base(key, value)
        {

        }

        protected override string ComparisonQualifier
        {
            get { return ">="; }
        }
    }

    public class Like<T> : ComparisonExpr<T>
    {
        public Like(string key, object value)
            : base(key, value)
        {

        }

        protected override string ComparisonQualifier
        {
            get { return " LIKE "; }
        }
    }
}
