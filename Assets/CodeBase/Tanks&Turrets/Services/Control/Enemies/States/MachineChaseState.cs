using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeBase.Tanks_Turrets.Data.Entities;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services.Control.Enemies.States
{
    public sealed class MachineChaseState : IEnemyMachineState
    {
        private readonly TimeSpan _observeChaseDelay = new(hours: 0, minutes: 0, seconds: 1);
        
        private readonly AISteering _steering;
        private IEnumerable<Vector3> _path;
        
        private int _noVisionChaseTimer = 0;
        
        public MachineChaseState(AISteering steering)
        {
            _steering = steering;
        }
        
        public void Enter(CancellationToken exitToken)
            => StartChase(exitToken).Forget();
        
        private async UniTaskVoid StartChase(CancellationToken exitToken)
        {
            if(_steering.Settings.Enemy is Turret) return;
            
            ObserveChaseExit(exitToken).Forget();
            while (exitToken.IsCancellationRequested is false)
            {
                _path = _steering.Mover.GetPathToPlayer(_steering.Settings).SkipLast(1);
                await _steering.Mover.MoveOnPath(_steering.Settings, _path, exitToken);
                await UniTask.Yield();
            }
        }
        
        public void OnCanSeePlayer()
        {
            _noVisionChaseTimer = 0;
            
            var tower = _steering.Settings.Enemy.Tower;
            var towerPosition = tower.position;
            var playerPos =_steering.Settings.Player.transform.position;

            var direction = new Vector2(playerPos.x - towerPosition.x, playerPos.y - towerPosition.y);
            
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            tower.rotation = Quaternion.Slerp(tower.rotation, targetRotation, _steering.TowerRotationSpeed);
            
            var hits = Physics2D.RaycastAll(towerPosition, direction.normalized, distance: Vector3.Distance(towerPosition, playerPos));
            var isReadyTyFire = hits.Any(h => h.transform == _steering.Settings.Player.transform);
            if (isReadyTyFire)
            {
                _steering.Settings.Enemy.Fire();
            }
        }

        private async UniTaskVoid ObserveChaseExit(CancellationToken exitToken)
        {
            while (exitToken.IsCancellationRequested is false)
            {
                _noVisionChaseTimer++; 
                await UniTask.Delay(_observeChaseDelay, cancellationToken: exitToken);
                
                var isTimeUp = _noVisionChaseTimer > _steering.ChargeTime;
                if (isTimeUp)
                {
                    if(_steering.Settings.Enemy is Tank)
                        _steering.EnterStare<TankPatrolState>();
                    if(_steering.Settings.Enemy is Turret)
                        _steering.EnterStare<MachineChaseState>();
                }
            }
        }
    }
}