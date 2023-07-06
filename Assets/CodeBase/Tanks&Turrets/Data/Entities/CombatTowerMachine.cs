using System;
using CodeBase.CombatReadinessSystems.Behaviors;
using CodeBase.Infrastructure.OneEntity;
using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Services._FireSystem;
using CodeBase.Tanks_Turrets.Visual;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Data.Entities
{
    public abstract class CombatTowerMachine : GameEntity, IHitable, ICanFire
    {
        [SerializeField] private Transform _tower;
        [SerializeField] private Transform _muzzleEdge;
        [SerializeField] private HeathBar _heathBar;
        

        private FireSystem _fireSystem;
        private float _lastFireTime;

        public Transform Tower => _tower;
        
        public CombatReadinessConfiguration CombatReadinessConfiguration { get; private set; }

        public float HeathAmount { get; set; }
        public float MaxHeathAmount { get; set; }
        public float AttackAmount { get; set; }

        protected void Constructor(FireSystem fireSystem, CombatReadinessConfiguration combatReadinessConfiguration)
        {
            _heathBar.Init(this);
            _fireSystem = fireSystem;
            CombatReadinessConfiguration = combatReadinessConfiguration;
            HeathAmount = combatReadinessConfiguration.HealthAmount;
            AttackAmount = combatReadinessConfiguration.Damage;
            MaxHeathAmount = combatReadinessConfiguration.HealthAmount;
        }

        public virtual void Fire()
        {
            var isRearmed = Time.time > _lastFireTime + CombatReadinessConfiguration.RearmTime;
            if (isRearmed is false) return;

            _fireSystem.Fire(CombatReadinessConfiguration, _muzzleEdge, transform);
            _lastFireTime = Time.time;
        }
    }
}