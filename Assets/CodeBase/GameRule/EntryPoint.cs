using UnityEngine;
using VContainer;

namespace CodeBase.GameRule
{
    public sealed class EntryPoint : MonoBehaviour
    {
        [Inject] private GameCycle _gameCycle;
        
        public void Start()
            => _gameCycle.Enter();
    }
}