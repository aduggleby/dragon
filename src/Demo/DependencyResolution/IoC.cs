// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


using Demo.Controllers;
using Dragon.Context;
using Dragon.Context.ReverseIPLookup;
using Dragon.Context.Sessions;
using Dragon.Context.Sessions.ReverseIPLookup;
using Dragon.Context.Users;
using Dragon.Core.Configuration;
using Dragon.Interfaces;

namespace Demo.DependencyResolution {
    //public static class IoC {
    //    public static IContainer Initialize() {
    //        ObjectFactory.Initialize(x =>
    //                    {
    //                        x.Scan(scan =>
    //                                {
    //                                    scan.TheCallingAssembly();
    //                                    scan.WithDefaultConventions();
    //                                });
    //        //                x.For<IExample>().Use<Example>();
    //                        x.For<IPermissionStore>().Use<SqlPermissionStore>();
    //                        x.For<ISessionStore>().Use<SqlSessionStore>();
    //                        x.For<ISession>().Use<CookieSession>();
    //                        x.For<IUserStore>().Use<SqlUserStore>();
    //                        x.For<IReverseIPLookupService>().Use<HostIpReverseLookupService>();
    //                        x.For<IConfiguration>().Use<ConfigurationManagerConfiguration>();
                            
    //                        x.FillAllPropertiesOfType<DragonContext>();
    //                        x.FillAllPropertiesOfType<ContextController>();
    //                    });
    //        return ObjectFactory.Container;
    //    }
    //}

}