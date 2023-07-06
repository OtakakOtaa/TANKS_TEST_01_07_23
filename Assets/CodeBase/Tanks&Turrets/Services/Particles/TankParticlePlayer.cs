using System.Linq;
using CodeBase.Infrastructure;
using CodeBase.Tanks_Turrets.Data;
using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Services._FireSystem;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services.Particles
{
    public sealed class TankParticlePlayer : IFireSystemSubscriber
    {
        private ObjectPool<Animator> _smokePool;
        private ObjectPool<Animator> _bulletExplosionPool;
        private ObjectPool<Animator> _tankExplosionPool;
        
        
        public async void OnFire(CombatReadinessConfiguration configurations, Vector3 position, Transform muzzle)
        {
            _smokePool ??= new ObjectPool<Animator>(configurations.BulletSmoke);
            var smoke = _smokePool.Fetch(position);
            smoke.transform.parent = muzzle;
            await Play(smoke);
            
            smoke.transform.parent = null;
            _smokePool.Put(smoke);
        }

        public async void OnHit(CombatReadinessConfiguration configurations, Vector3 position)
        {
            _bulletExplosionPool ??= new ObjectPool<Animator>(configurations.BulletExplosion);
            var bullet = _bulletExplosionPool.Fetch(position);
            await Play(bullet);
            _bulletExplosionPool.Put(bullet);
        }

        public async void OnTankExploded(CombatReadinessConfiguration configurations, Vector3 position)
        {
            _tankExplosionPool ??= new ObjectPool<Animator>(configurations.TankExplosion);
            var tank = _tankExplosionPool.Fetch(position);
            await Play(tank);
            _tankExplosionPool.Put(tank);
        }

        private async UniTask Play(Animator animator)
        {
            animator.Rebind();
            animator.Play(animator.runtimeAnimatorController.animationClips.First().name);
            await UniTask.Delay((int)animator.runtimeAnimatorController.animationClips.First().length * 100);
        }
    }
}