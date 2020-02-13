using Domain.Abstract;
using Domain.Concrete;
using Domain.Entities;
using Moq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }

        private void AddBindings()
        {
            kernel.Bind<IRateRepository>().To<EFRateRepository>();
            kernel.Bind<IUserRepository>().To<EFUserRepository>();
            kernel.Bind<IMrrRepository>().To<EFMrrRepository>();
            kernel.Bind<IBtcRepository>().To<EFBtcRepository>();
            kernel.Bind<ISpRepository>().To<EFSpRepository>();
            kernel.Bind<IMinearRepository>().To<EFMinearRepository>();
            kernel.Bind<IConstRepository>().To<EFConstRepository>();
            kernel.Bind<IPayoutRepository>().To<EFPayoutRepository>();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }
}