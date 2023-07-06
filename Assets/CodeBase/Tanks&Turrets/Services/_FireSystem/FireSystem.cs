using System.Threading;
using CodeBase.CombatReadinessSystems.Behaviors;
using CodeBase.Infrastructure;
using CodeBase.Tanks_Turrets.Data;
using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Data.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services._FireSystem
{
    public sealed class FireSystem
    {
        private readonly IFireSystemSubscriber _subscriber;
        private readonly ObjectPool<Bullet> _bulletPool;

        public FireSystem(Bullet prefab, IFireSystemSubscriber subscriber)
        {
            _subscriber = subscriber;
            _bulletPool = new ObjectPool<Bullet>(prefab);
        }
        
        public async void Fire(CombatReadinessConfiguration configurations, Transform origin, Transform tank)
        {
            var bulletHitAction = InitBullet(origin, tank, out var moveBulletState, out var target, out var bullet);
            GameObject targetObject = null;
            bullet.HitTheTarget += OnBulletHitTarget; 
            bullet.Missed += OnBulletMiss;
            
            FireBullet(configurations, origin, moveBulletState, bullet);
            
            var isHit = await bulletHitAction.Task;
            if (isHit)
            {
                target.Hit(configurations.Damage);
                _subscriber.OnHit(configurations, bullet.transform.position);
                
                if (target.IsDie)
                {
                    _subscriber.OnTankExploded(configurations, targetObject.transform.position);
                    Object.Destroy(targetObject);
                }
            }
            
            bullet.HitTheTarget -= OnBulletHitTarget; 
            bullet.Missed -= OnBulletMiss;
            _bulletPool.Put(bullet);
            
            void OnBulletHitTarget(IHitable hitable, GameObject gameObject)
            {
                target = hitable;
                targetObject = gameObject;
                moveBulletState.Cancel();
                bulletHitAction.TrySetResult(true);
            }

            void OnBulletMiss()
            {
                moveBulletState.Cancel();
                bulletHitAction.TrySetResult(false);
            }
        }

        private UniTaskCompletionSource<bool> InitBullet(Transform origin, Transform tank,
            out CancellationTokenSource moveBulletState, out IHitable target, out Bullet bullet)
        {
            UniTaskCompletionSource<bool> bulletHit = new();
            moveBulletState = new();
            target = null;

            bullet = _bulletPool.Fetch(origin.position);
            bullet.transform.rotation = origin.rotation;
            bullet.Parent = tank.gameObject;
            
            return bulletHit;
        }

        private void FireBullet(CombatReadinessConfiguration configurations, Transform origin,
            CancellationTokenSource moveBulletState, Bullet bullet)
        {
            MoveBullet(moveBulletState.Token, bullet, origin.right, configurations.BulletSpeed).Forget();
            _subscriber.OnFire(configurations, bullet.transform.position, origin);
        }

        private async UniTaskVoid MoveBullet(CancellationToken token, Bullet bullet, Vector3 direction, float speed)
        {
            while (token.IsCancellationRequested is false)
            {
                bullet.transform.position += direction * speed * Time.deltaTime;
                await UniTask.Yield(token);
            }
        }
    }
}