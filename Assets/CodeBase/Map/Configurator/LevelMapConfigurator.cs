using CodeBase.Configurations;
using CodeBase.Infrastructure.Di;
using CodeBase.Map.Services;
using CodeBase.Map.Services.Core;
using UnityEngine;
using VContainer;

namespace CodeBase.Map.Configurator
{
    public sealed class LevelMapConfigurator 
        : ConfigRegisterScope<(GameConfiguration, LevelMap.MapLayers, Transform)>
    {
        public LevelMapConfigurator(IContainerBuilder builder) : base(builder) { }
        
        public override void Configure((GameConfiguration, LevelMap.MapLayers, Transform) @params)
        {
            Builder.Register<LevelMapGenerator>(Lifetime.Singleton);
            
            Builder.Register<MapNavigator>(Lifetime.Singleton);

            Builder.Register<LevelMap>(Lifetime.Singleton)
                .WithParameter(@params.Item1)
                .WithParameter(@params.Item2)
                .WithParameter(@params.Item3);
        }
    }
}