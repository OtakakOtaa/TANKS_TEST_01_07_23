using System.Linq;
using System.Threading;
using CodeBase.GameRule.GameStates._GameplayInitState;
using CodeBase.Infrastructure;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace CodeBase.GameRule.GameStates
{
    public sealed class GameplayState : IGameState
    {
        private GameplayInitState _gameplayInitState;

        public GameplayInitState FetchData(GameplayInitState gameplayInitState)
            => _gameplayInitState = gameplayInitState;

        public async void Enter()
        {
            CancellationTokenSource tokenSource = new ();
            ObserveEndGame(tokenSource.Token).Forget();
            ObserveRestart(tokenSource).Forget();
        }

        private async UniTaskVoid ObserveEndGame(CancellationToken token)
        {
            await WaitPlayerDie();
            if(token.IsCancellationRequested) return;
            
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            UnityEngine.Application.Quit();
        }

        private async UniTaskVoid ObserveRestart(CancellationTokenSource tokenSource)
        {
            await WaitUntilPlayerKillsAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            tokenSource.Cancel();
        }

        private async UniTask WaitPlayerDie()
        {
            UniTaskCompletionSource playerDied = new ();
            _gameplayInitState.Player.Destroyed += () => playerDied.TrySetResult();
            await playerDied.Task;
        }

        private async UniTask WaitUntilPlayerKillsAll()
        {
            await UniTask.WaitUntil(() => _gameplayInitState.EnemiesTanks.Count() + 
                                          _gameplayInitState.EnemiesTurrets.Count() is 0);
        }
    }
}