using CodeBase.Configurations;
using CodeBase.GameRule.GameStates;
using CodeBase.GameRule.GameStates._GameplayInitState;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Di;

namespace CodeBase.GameRule
{
    public sealed class GameplayCycle : IGameState
    {
        private readonly SceneScopeProvider _scopeProvider;
        private readonly GameConfiguration _gameConfiguration;

        public GameplayCycle(SceneScopeProvider sceneScopeProvider, GameConfiguration gameConfiguration)
        {
            _scopeProvider = sceneScopeProvider;
            _gameConfiguration = gameConfiguration;
        }

        public void Enter()
        {
            var initialize = GetGameplayInitState();
            initialize.Enter();
            
            var gameplay = GetGameplayState();
            gameplay.FetchData(initialize);
            gameplay.Enter();
        }

        private GameplayInitState GetGameplayInitState() 
        {
            var state = _scopeProvider.Scope().CreateInstanceFromContainer<GameplayInitState>();
            state.FetchConfiguration(_gameConfiguration);
            return state;
        }

        private GameplayState GetGameplayState()
        {
            var state = _scopeProvider.Scope().CreateInstanceFromContainer<GameplayState>();
            return state;
        }
    }
}