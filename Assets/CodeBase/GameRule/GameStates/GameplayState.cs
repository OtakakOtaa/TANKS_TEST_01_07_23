using System.Threading;
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
            UniTaskCompletionSource playerDied = new ();
            _gameplayInitState.Player.Destroyed += () => playerDied.TrySetResult();
            
            await playerDied.Task;
            if(token.IsCancellationRequested) return;
            
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
            UnityEngine.Application.Quit();
        }

        private async UniTaskVoid ObserveRestart(CancellationTokenSource tokenSource)
        {
            await UniTask.WaitUntil(() => _gameplayInitState.EnemyTanks.Count + _gameplayInitState.EnemyTurrets.Count is 0);
            tokenSource.Cancel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}