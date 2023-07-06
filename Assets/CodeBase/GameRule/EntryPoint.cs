using UnityEngine;
using VContainer;

namespace CodeBase.GameRule
{
    public sealed class EntryPoint : MonoBehaviour
    {
        [Inject] private GameplayCycle _gameplayCycle;
        
        public void Start()
            => _gameplayCycle.Enter();
    }
}