using System;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace Demo.DependencyResolution
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        protected override IController
            GetControllerInstance(RequestContext requestContext,
                Type controllerType)
        {
            try
            {
                if ((requestContext == null) || (controllerType == null))
                    return null;

                return (Controller) ObjectFactory.GetInstance(controllerType);
            }
            catch (StructureMapException)
            {
                Debug.WriteLine(ObjectFactory.WhatDoIHave());
                throw;
            }
        }
    }
}