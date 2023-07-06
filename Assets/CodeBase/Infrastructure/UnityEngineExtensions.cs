using UnityEngine;

namespace CodeBase.Infrastructure
{
    public static class UnityEngineExtensions
    {
        public static TBehavior AddUniqueBehavior<TBehavior>(this GameObject gameObject)
            where TBehavior : Behaviour
            => gameObject.TryGetComponent<TBehavior>(out var attacker)
                ? attacker
                : gameObject.AddComponent<TBehavior>();
    }
}