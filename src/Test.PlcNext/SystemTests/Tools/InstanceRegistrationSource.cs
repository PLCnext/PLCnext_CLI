#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac.Core;
using Autofac.Core.Activators.ProvidedInstance;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

namespace Test.PlcNext.SystemTests.Tools
{
    public class InstancesRegistrationSource : IRegistrationSource
    {
        private readonly List<object> instances = new List<object>();

        public void AddInstance(object instance)
        {
            instances.Add(instance);
        }

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            return instances.Where(i => (service as IServiceWithType)?.ServiceType.IsInstanceOfType(i) == true)
                            .Select(i => new ComponentRegistration(Guid.NewGuid(),
                                                                   new ProvidedInstanceActivator(i),
                                                                   new RootScopeLifetime(),
                                                                   InstanceSharing.Shared,
                                                                   InstanceOwnership.ExternallyOwned,
                                                                   new[] {service},
                                                                   new Dictionary<string, object>()));
        }

        public bool IsAdapterForIndividualComponents => false;
    }
}
