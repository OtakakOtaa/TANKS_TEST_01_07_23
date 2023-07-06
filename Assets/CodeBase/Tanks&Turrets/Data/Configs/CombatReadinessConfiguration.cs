using System;
using CodeBase.Tanks_Turrets.Data.Entities;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Data.Configs
{
    [Serializable] public sealed class CombatReadinessConfiguration
    {
        [Header("Main")]
        [SerializeField] private float _damage;
        [SerializeField] private float _healthAmount;
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private float _rearmTime;
        [SerializeField] private float _towerRotationSpeed;

        [Header("AI")]
        [SerializeField] private float _scope;
        [SerializeField] private float _chargeTime;

        [Header("Assets")]
        [SerializeField] private Bullet _bullet;
        [SerializeField] private Animator _bulletExplosion;
        [SerializeField] private Animator _tankExplosion;
        [SerializeField] private Animator _bulletSmoke;
            
        public float Damage => _damage;
        public float HealthAmount => _healthAmount;
        public float BulletSpeed => _bulletSpeed;
        public Bullet Bullet => _bullet;
        public Animator BulletExplosion => _bulletExplosion;
        public Animator TankExplosion => _tankExplosion;
        public Animator BulletSmoke => _bulletSmoke;
        public float RearmTime => _rearmTime;
        public float Scope => _scope;
        public float ChargeTime => _chargeTime;
        public float TowerRotationSpeed => _towerRotationSpeed;
    }
}