using CodeBase.Configurations;
using CodeBase.Infrastructure.Di;
using VContainer;

namespace CodeBase.GameRule.Configurator
{
    public sealed class GameRuleConfigurator : ConfigRegisterScope<GameConfiguration>
    {
        public GameRuleConfigurator(IContainerBuilder builder) : base(builder) { }

        public override void Configure(GameConfiguration gameConfiguration)
        {
            Builder.Register<SceneScopeProvider>(Lifetime.Singleton);
            
            Builder.Register<GameCycle>(Lifetime.Singleton)
                .WithParameter(gameConfiguration);
        }
    }
}