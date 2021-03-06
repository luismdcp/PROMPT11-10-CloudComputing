﻿using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;

namespace CloudNotes.Infrastructure.DependencyInjection
{
    public class StructureMapDependencyResolver : IDependencyResolver
    {
        private readonly IContainer _container;

        public StructureMapDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null) return null;

            try
            {
                if (serviceType.IsAbstract || serviceType.IsInterface)
                {
                    return _container.TryGetInstance(serviceType);
                }
                else
                {
                    return _container.GetInstance(serviceType);
                }
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances<object>().Where(s => s.GetType() == serviceType);
        }
    }
}