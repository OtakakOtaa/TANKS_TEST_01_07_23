using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services.Control.Enemies.States
{
    public sealed class TankPatrolState : IEnemyMachineState
    {
        private readonly TimeSpan _restTimeBetweenPatrols = new (hours:0, minutes: 0, seconds: 5);

        private readonly AISteering _steering;
        private IEnumerable<Vector3> _path;

        public TankPatrolState(AISteering steering)
        {
            _steering = steering;
        }
        
        public void Enter(CancellationToken exitToken)
            => StartPatrol(exitToken).Forget();

        private async UniTaskVoid StartPatrol(CancellationToken exitToken)
        {
            AlignTower(exitToken).Forget();
            while (exitToken.IsCancellationRequested is false)
            {
                _path = await _steering.Mover.GetPathToRandomPoint(_steering.Settings, exitToken);
                await _steering.Mover.MoveOnPath(_steering.Settings, _path, exitToken);
                await UniTask.Delay(_restTimeBetweenPatrols, cancellationToken: exitToken);
            }
        }

        private async UniTaskVoid AlignTower(CancellationToken exitToken)
        {
            while (exitToken.IsCancellationRequested is false)
            {
                var tankDirection = _steering.Settings.Enemy.transform.right;
                var tower = _steering.Settings.Enemy.Tower;
                var towerPosition = tower.position;
                var forwardPos = tankDirection + towerPosition;

                var direction = new Vector2(forwardPos.x - towerPosition.x, forwardPos.y - towerPosition.y);
                
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
                tower.rotation = Quaternion.Slerp(tower.rotation, targetRotation, _steering.TowerRotationSpeed);

                await UniTask.Yield();

                var isAligned = tower.rotation.z is 0;
                if(isAligned)
                    return;
            }
        }
        
        public void OnCanSeePlayer()
            => _steering.EnterStare<MachineChaseState>();
        
    }
}