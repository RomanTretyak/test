using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace test.Infrastructure
{
    using Ninject;
    using Models;

    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel kernel;

        public NinjectControllerFactory()
        {
            this.kernel = new StandardKernel();
        }

        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            return this.kernel.Get(controllerType) as IController;
        }

        private void AddBindings()
        {
            var linqHelper = new BaseLinq();

            this.kernel.Bind<BaseLinq>().To<BaseLinq>().WithConstructorArgument("linqHelper", linqHelper);
        }
    }
}