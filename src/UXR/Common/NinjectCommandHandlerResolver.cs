using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ninject;
using UXI.CQRS.Commands;

namespace UXR.Common
{
    public class NinjectCommandHandlerResolver : ICommandHandlerResolver
    {
        private readonly IKernel _kernel;
        public NinjectCommandHandlerResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public ICommandHandler<TCommand> Resolve<TCommand>() where TCommand : ICommand
        {
            return _kernel.Get<ICommandHandler<TCommand>>();
        }
    }
}
