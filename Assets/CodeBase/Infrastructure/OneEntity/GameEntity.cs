using System;
using UnityEngine;

namespace CodeBase.Infrastructure.OneEntity
{
    public abstract class GameEntity : MonoBehaviour
    {
        public event Action Destroyed;

        public void OnDestroy()
            => Destroyed?.Invoke();
    }

    public abstract class GameEntity<TBehavior> : MonoBehaviour
    {
        public event Action Destroyed;

        public void OnDestroy()
            => Destroyed?.Invoke();
    }

    public abstract class GameEntity<TBehavior1, TBehavior2> : MonoBehaviour
    {
        public event Action Destroyed;

        public void OnDestroy()
            => Destroyed?.Invoke();
    }

    public abstract class GameEntity<TBehavior1, TBehavior2, TBehavior3> : MonoBehaviour
    {
        public event Action Destroyed;

        public void OnDestroy()
            => Destroyed?.Invoke();
    }

    public abstract class GameEntity<TBehavior1, TBehavior2, TBehavior3, TBehavior4> : MonoBehaviour
    {
        public event Action Destroyed;

        public void OnDestroy()
            => Destroyed?.Invoke();
    }
}