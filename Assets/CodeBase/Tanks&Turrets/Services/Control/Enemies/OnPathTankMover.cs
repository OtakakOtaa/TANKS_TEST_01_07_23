using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeBase.Tanks_Turrets.Data.Entities;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services.Control.Enemies
{
    public sealed class OnPathTankMover
    {
        private readonly TimeSpan _pathFindDelay = new (hours:0, minutes: 0, seconds: 5);

        public async UniTask<IEnumerable<Vector3>> GetPathToRandomPoint(MachineControlSettings settings, 
            CancellationToken exitToken)
        {
            while (exitToken.IsCancellationRequested is false)
            {
                var targetPos = settings.LevelMap.GetRandomFreePlace();
                var selfMapPos = settings.LevelMap.GetWorldPosToMap(settings.Enemy.transform.position);
                
                if (selfMapPos is null) throw new Exception();
                var selfPos = (Vector3Int) selfMapPos;

                var path = settings.MapNavigator.TryFindPath(selfPos, targetPos, settings.LevelMap);
                if (path is not null) 
                    return path.Select(settings.LevelMap.GetTrueCellPosition);
                
                await UniTask.Delay(_pathFindDelay, cancellationToken: exitToken);
            }
            return null;
        }

        public IEnumerable<Vector3> GetPathToPlayer(MachineControlSettings settings)
        {
            var targetPos = settings.LevelMap.GetWorldPosToMap(settings.Player.transform.position);
            var selfMapPos = settings.LevelMap.GetWorldPosToMap(settings.Enemy.transform.position);
            
            if (selfMapPos is null || targetPos is null) throw new Exception();
            var selfPos = (Vector3Int) selfMapPos;

            var path = settings.MapNavigator.TryFindPath(selfPos, (Vector3Int)targetPos, settings.LevelMap);
            if (path is null) throw new Exception();
            
            return path.Select(settings.LevelMap.GetTrueCellPosition);
        }
        
        public async UniTask MoveOnPath(MachineControlSettings settings, IEnumerable<Vector3> path, 
            CancellationToken exitToken)
        {
            foreach (var node in path)
                await ArriveToPathNode(settings, node, exitToken);
        }
        
        private async UniTask ArriveToPathNode(MachineControlSettings settings, Vector3 targetNode, 
            CancellationToken exitToken)
        {
            while (exitToken.IsCancellationRequested is false)
            {
                var enemyTank = settings.Enemy as Tank;
                var transform = enemyTank!.transform;
                var moveDirection = (targetNode - transform.position).normalized;
                var needTurn = moveDirection != transform.right && moveDirection != Vector3.zero;
                if (needTurn)
                    await MakeTurn(settings, moveDirection, exitToken);
                
                var pos = CalculateMovement(targetNode, enemyTank);
                settings.Enemy.transform.position = pos;

                var isArrivedToPathNode = pos == CalculateMovement(targetNode, enemyTank);
                if (isArrivedToPathNode) return;

                await UniTask.Yield();
            }
        }
        
        private async UniTask MakeTurn(MachineControlSettings settings, Vector3 direction, CancellationToken exitToken)
        {
            var transform = settings.Enemy.transform;
            
            while (exitToken.IsCancellationRequested is false)
            {
                transform.rotation = CalculateRotation();
                var tankTurned = transform.rotation == CalculateRotation();
                if (tankTurned) return;
                
                await UniTask.Yield();
            }

            Quaternion CalculateRotation()
            {
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
                var enemyTank = settings.Enemy as Tank;
                var rotationSpeed = enemyTank!.TankConfiguration.Motion.RotationSpeed * Time.deltaTime;
                return Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            }
        }
        
        private Vector3 CalculateMovement(Vector3 target, Tank tank)
            => Vector3.MoveTowards(tank.transform.position, target, tank.TankConfiguration.Motion.MoveSpeed * Time.deltaTime);
        
    }
}