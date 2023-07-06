using System;
using System.Linq;
using System.Threading;
using CodeBase.Infrastructure.OneEntity;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.Data;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.States;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services.Control.Enemies
{
    public sealed class AISteering : IDisposable
    {
        private IEnemyMachineState _state;
        
        public MachineControlSettings Settings { get; }
        public OnPathTankMover Mover { get; }
        public float Scope => Settings.Enemy.CombatReadinessConfiguration.Scope;
        public float ChargeTime => Settings.Enemy.CombatReadinessConfiguration.ChargeTime;

        private CancellationTokenSource _exitStateLocator;

        public AISteering(OnPathTankMover mover, MachineControlSettings settings)
        {
            Mover = mover;
            Settings = settings;
            settings.Enemy.Destroyed += Dispose;
        }

        public void Dispose()
        {
            _exitStateLocator?.Cancel();
            Settings.Enemy.Destroyed -= Dispose;
        }

        public void EnterStare<TState>() where TState : class, IEnemyMachineState
        {
            _exitStateLocator?.Cancel();
            _exitStateLocator = new CancellationTokenSource();
            
            if (typeof(TState) == typeof(TankPatrolState))
                _state = new TankPatrolState(this);
            if(typeof(TState) == typeof(MachineChaseState))
                _state = new MachineChaseState(this);

            _state?.Enter(_exitStateLocator.Token);
            ObserveCanSeePlayer().Forget();
        }


        private async UniTaskVoid ObserveCanSeePlayer()
        {
            while (_exitStateLocator.IsCancellationRequested is false)
            {
                if (CanSeePlayerTank())
                    _state.OnCanSeePlayer();
                await UniTask.Yield();
            }
        }

        public float TowerRotationSpeed
            => Settings.Enemy.CombatReadinessConfiguration.TowerRotationSpeed * Time.deltaTime;
        
        private bool CanSeePlayerTank()
        {
            var player = Settings.Player.transform.position;
            var enemy = Settings.Enemy.transform.position;
            
            var direction = player - enemy;
            var distance = Vector3.Distance(player, enemy);

            if (distance > Scope)
                return false;

            var hits = Physics2D.RaycastAll(enemy, direction.normalized, distance);
            return !hits.Any(hit => hit.transform.TryGetComponent<GameEntity>(out _) is false);
        }
    }
}