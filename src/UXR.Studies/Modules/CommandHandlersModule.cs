using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.CQRS.Commands;
using UXR.Studies.Models;
using UXR.Studies.Models.Commands;

namespace UXR.Studies.Modules
{
    public class CommandHandlersModule : NinjectModule
    {
        private void BindTransactionDecoratedCommand<TCommand, TCommandHandlerImpl>()
            where TCommandHandlerImpl : class, ICommandHandler<TCommand>
            where TCommand : ICommand
        {
            Bind<ICommandHandler<TCommand>>().To<TCommandHandlerImpl>()
                .WhenInjectedInto<TransactionCommandHandlerDecorator<TCommand, IStudiesDbContext>>();
            Bind<ICommandHandler<TCommand>>().To<TransactionCommandHandlerDecorator<TCommand, IStudiesDbContext>>()
                .InTransientScope();
        }

        public override void Load()
        {
            BindTransactionDecoratedCommand<CreateGroupCommand, GroupCommandsHandler>();
            BindTransactionDecoratedCommand<EditGroupCommand, GroupCommandsHandler>();
            BindTransactionDecoratedCommand<DeleteGroupCommand, GroupCommandsHandler>();

            BindTransactionDecoratedCommand<CreateNodesCommand, NodeCommandsHandler>();
            BindTransactionDecoratedCommand<DeleteNodeCommand, NodeCommandsHandler>();

            BindTransactionDecoratedCommand<CreateProjectCommand, ProjectCommandsHandler>();
            BindTransactionDecoratedCommand<EditProjectCommand, ProjectCommandsHandler>();
            BindTransactionDecoratedCommand<DeleteProjectCommand, ProjectCommandsHandler>();

            BindTransactionDecoratedCommand<CreateSessionCommand, SessionCommandsHandler>();
            BindTransactionDecoratedCommand<EditSessionCommand, SessionCommandsHandler>();
            BindTransactionDecoratedCommand<DeleteSessionCommand, SessionCommandsHandler>();

            BindTransactionDecoratedCommand<CreateSessionTemplateCommand, SessionTemplateCommandsHandler>();
            BindTransactionDecoratedCommand<DeleteSessionTemplateCommand, SessionTemplateCommandsHandler>();

            BindTransactionDecoratedCommand<UpdateNodeStatusCommand, NodeStatusCommandsHandler>();
        }
    }
}
