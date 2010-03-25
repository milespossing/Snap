﻿using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;

namespace Snap.CastleWindsor
{
    /// <summary>
    /// Facility for Castle interceptor registration
    /// </summary>
    public class CastleAspectFacility : AbstractFacility
    {
        /// <summary>
        /// Initializes the facility.
        /// </summary>
        protected override void Init()
        {
            //_container = (CastleAspectContainer)Kernel["CastleAspectContainer"];
            Kernel.ComponentRegistered += KernelComponentRegistered;
        }
        /// <summary>
        /// Registers interceptors with the target type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="handler">The handler.</param>
        private void KernelComponentRegistered(string key, IHandler handler)
        {
            // Ignore any types implementing IAttributeInterceptor or IInterceptor
            if(handler.Service.GetInterfaces().Any(i => i.FullName.Contains("Snap.IAttributeInterceptor")
                || i.FullName.Contains("IInterceptor")))
            {
                return;
            }

            var proxy = (MasterProxy)Kernel[typeof (MasterProxy)];

            handler.ComponentModel.Interceptors.AddIfNotInCollection(new InterceptorReference(typeof(MasterProxy)));

            for (var i = 1; i < proxy.Configuration.Interceptors.Count; i++)
            {
                handler.ComponentModel.Interceptors.Add(new InterceptorReference(typeof(PseudoInterceptor)));
            }
        }
    }
}