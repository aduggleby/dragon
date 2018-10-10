using System;
using System.Collections.Generic;
using System.Text;
using Dragon.Data.Interfaces;
using AutoMapper;
using System.Reflection;
using System.Linq;
using Dragon.Data.Attributes;
using Dragon.Data.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Dragon.CPRX
{
    public class CPRSingleton
    {
        private static object s_syncRoot = new Object();
        private static volatile Dictionary<Type, object> s_instances = new Dictionary<Type, object>();

        public static T Instance<T>()
            where T : class
        {

            if (!s_instances.ContainsKey(typeof(T)))
            {
                lock (s_syncRoot)
                {
                    if (!s_instances.ContainsKey(typeof(T)))
                    {
                        s_instances.Add(typeof(T), Activator.CreateInstance<T>());
                    }
                }
            }
            return (T)s_instances[typeof(T)];
        }
    }

}
