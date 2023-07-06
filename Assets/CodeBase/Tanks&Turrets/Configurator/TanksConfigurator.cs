using CodeBase.Configurations;
using CodeBase.Infrastructure.Di;
using CodeBase.Tanks_Turrets.Services._FireSystem;
using CodeBase.Tanks_Turrets.Services.Control.Enemies;
using CodeBase.Tanks_Turrets.Services.Control.Player;
using CodeBase.Tanks_Turrets.Services.Factory;
using CodeBase.Tanks_Turrets.Services.Particles;
using VContainer;

namespace CodeBase.Tanks_Turrets.Configurator
{
    public sealed class TanksConfigurator : ConfigRegisterScope<(GameConfiguration, PlayerInputHandler)>
    {
        public TanksConfigurator(IContainerBuilder builder) : base(builder) { }
        
        public override void Configure((GameConfiguration, PlayerInputHandler) @params)
        {
            Builder.Register<TankParticlePlayer>(Lifetime.Singleton)
                .As<IFireSystemSubscriber>()
                .AsSelf();
            
            Builder.Register<TankAndTurretsFactory>(Lifetime.Singleton)
                .WithParameter(@params.Item1);
    
            Builder.Register<FireSystem>(Lifetime.Singleton)
                .WithParameter(@params.Item1.PlayerTankConf.CombatReadiness.Bullet);

            Builder.Register<OnPathTankMover>(Lifetime.Singleton);
            
            Builder.RegisterInstance(@params.Item2);
        }
    }
}