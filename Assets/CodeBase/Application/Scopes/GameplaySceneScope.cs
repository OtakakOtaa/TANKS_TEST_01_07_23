using CodeBase.Configurations;
using CodeBase.GameRule.Configurator;
using CodeBase.Map.Configurator;
using CodeBase.Map.Services.Core;
using CodeBase.Tanks_Turrets.Configurator;
using CodeBase.Tanks_Turrets.Services.Control.Player;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Application.Scopes
{
    public sealed class GameplaySceneScope : LifetimeScope
    {
        [Header("Links")]
        [SerializeField] private GameConfiguration _gameConfiguration;
        [SerializeField] private LevelMap.MapLayers _mapLayers;
        [SerializeField] private PlayerInputHandler _playerInputHandler;
        [SerializeField] private Transform _mapRoot;

        protected override void Configure(IContainerBuilder builder)
        {
            new TanksConfigurator(builder)
                .Configure((_gameConfiguration, _playerInputHandler));
            
            new GameRuleConfigurator(builder)
                .Configure(_gameConfiguration);
            
            new LevelMapConfigurator(builder)
                .Configure((_gameConfiguration, _mapLayers, _mapRoot));
        }
    }
}