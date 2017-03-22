using AgentTransferBot.Scorable;
using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgentTransferBot
{
    public class AgentModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<InMemoryAgentStore>()
                .Keyed<IAgentProvider>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<AgentService>()
                .As<IAgentService>()
                .As<IUserToAgent>()
                .As<IAgentToUser>()
                .InstancePerLifetimeScope();

            builder.RegisterType<AgentToUserScorable>()
                .As<IScorable<IMessageActivity, double>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TransferToAgentScorable>()
                .As<IScorable<IMessageActivity, double>>()
                .InstancePerLifetimeScope();
        }
    }
}
