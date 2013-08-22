using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Dragon.CPR.Interfaces;

namespace Dragon.CPR
{
    public class PersistableSetup : IDisposable
    {
        public SortedSet<string> CreatedTables { get; private set; }
        public SortedSet<string> DroppedTables { get; private set; }
        
        public SortedSet<string> Errors { get; private set; }

        private ISetupRepository m_setupRepository;
        private IDropRepository m_dropRepository;

        public PersistableSetup(ISetupRepository setupRep, IDropRepository dropRep)
        {
            CreatedTables = new SortedSet<string>();
            DroppedTables = new SortedSet<string>();
            Errors = new SortedSet<string>();

            m_setupRepository = setupRep;
            m_dropRepository = dropRep;
        }

        public void EnsureTablesForDerivingFrom<TBase>()
        {
            GetTypesForDerivingFrom<TBase>().ToList().ForEach(EnsureTable);
        }

        public void DropTablesForDerivingFrom<TBase>()
        {
            GetTypesForDerivingFrom<TBase>().ToList().ForEach(DropTable);
        }

        private IEnumerable<Type> GetTypesForDerivingFrom<TBase>()
        {
            return Dragon.CPR.Config.Assemblies.SelectMany(x=>x.GetTypes())
                .Where(t =>
                    t.IsClass && !t.IsAbstract && !t.IsNestedPublic &&
                    !(t.IsAbstract && t.IsSealed) /* => IsStatic */&&
                    !t.IsDefined(typeof (CompilerGeneratedAttribute), false)
                    /* eg Extension class */&&
                    typeof(TBase).IsAssignableFrom(t));
        }

        public void EnsureTablesForNamespace(string @namespace)
        {
            IEnumerable<Type> types = GetTypesForNamespace(@namespace);

            types.ToList().ForEach(EnsureTable);
        }

        public void DropTablesForNamespace(string @namespace)
        {
            IEnumerable<Type> types = GetTypesForNamespace(@namespace);

            types.ToList().ForEach(DropTable);
        }

        private IEnumerable<Type> GetTypesForNamespace(string @namespace)
        {
            return from t in Assembly.Load(@namespace).GetTypes()
                   where t.IsClass && !t.IsAbstract && !t.IsNestedPublic &&
                         !(t.IsAbstract && t.IsSealed) /* => IsStatic */ && 
                         !t.IsDefined (typeof (CompilerGeneratedAttribute), false) /* eg Extension class */ && 
                         t.Namespace.StartsWith(@namespace)
                   select t;
        }

        public void EnsureTable<T>() where T : class
        {
            try
            {
                m_setupRepository.EnsureTableExists<T>();
                CreatedTables.Add(typeof(T).Name);
            }
            catch (Exception e)
            {
                Errors.Add(string.Format(
                    "{0}: {1}",
                    typeof(T).Name,
                    e.Message
                ));
            }
            
        }

        public void DropTable<T>() where T : class
        {
            m_dropRepository.DropTableIfExists<T>();
            DroppedTables.Add(typeof(T).Name);
        }

        public void EnsureTable(Type t)
        {
            dynamic array = Array.CreateInstance(t, 0);
            Debug.WriteLine(t.FullName);
            EnsureTable(array);
        }

        public void DropTable(Type t)
        {
            dynamic array = Array.CreateInstance(t, 0);
            DropTable(array);
        }

        private void EnsureTable<T>(T[] array) where T : class
        {
            if (typeof(T) == typeof(object))
            {
                return;
            }

            EnsureTable<T>();
        }

        private void DropTable<T>(T[] array) where T : class
        {
            DropTable<T>();
        }

        public void DropTable(string name)
        {
            m_dropRepository.DropTableIfExists(name);
            DroppedTables.Add(name);
        }

        public void Dispose()
        {
            CreatedTables.Clear();
            DroppedTables.Clear();
            CreatedTables = null;
            DroppedTables = null;
        } 
    }
}
