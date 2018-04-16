using Autofac;
using Human_Hand_Off_Bot.HumanHandOff.Scorable;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace Human_Hand_Off_Bot.HumanHandOff
{
    /// <summary>註冊至 AutoFac 容器的模組</summary>
    public class HumanModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<HumanService>()
                .Keyed<IHumanService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            builder.RegisterType<HumanProvider>()
                .Keyed<IHumanProvider>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<AgentToUserScorable>()
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<UserToAgentScorable>()
                .As<IScorable<IActivity, double>>()
                .InstancePerLifetimeScope();
        }
    }
}