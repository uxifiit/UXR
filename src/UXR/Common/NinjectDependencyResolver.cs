using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;

namespace UXR.Common
{
    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver, System.Web.Mvc.IDependencyResolver
    {
        private readonly IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(new ChildKernel(this.kernel));
            //return new NinjectDependencyScope(this.kernel.BeginBlock());
        }
    }

    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;

        internal NinjectDependencyScope(IResolutionRoot resolver)
        {
            //Contract.Assert(resolver != null);

            this.resolver = resolver;
        }

        public void Dispose()
        {
            var disposable = this.resolver as IDisposable;
            if (disposable != null)
            {
                System.Diagnostics.Debug.WriteLine("disposing resolver");
                disposable.Dispose();
            }

            this.resolver = null;
        }

        public object GetService(Type serviceType)
        {
            try
            {

                if (this.resolver == null)
                {
                    throw new ObjectDisposedException("this", "This scope has already been disposed");
                }

                return this.resolver.TryGet(serviceType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                if (this.resolver == null)
                {
                    throw new ObjectDisposedException("this", "This scope has already been disposed");
                }

                return this.resolver.GetAll(serviceType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
