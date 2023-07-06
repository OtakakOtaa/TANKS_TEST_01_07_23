using System;
using CodeBase.CombatReadinessSystems.Behaviors;
using CodeBase.Infrastructure.OneEntity;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Data.Entities
{
    public sealed class Bullet : MonoBehaviour
    {
        public event Action<IHitable, GameObject> HitTheTarget;
        public event Action Missed;

        public GameObject Parent { get; set; }

        public void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject == Parent || col.gameObject.TryGetComponent<Bullet>(out _))
                return;
            
            if (col.gameObject.TryGetComponent<GameEntity>(out var entity) && entity is IHitable hitable)
                HitTheTarget?.Invoke(hitable, col.gameObject);
            else
                Missed?.Invoke();
        }
    }
}