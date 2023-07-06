using System;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using CodeBase.Configurations;
using CodeBase.Infrastructure;
using CodeBase.Map.Services;
using CodeBase.Map.Services.Core;
using CodeBase.Tanks_Turrets.Data.Entities;
using CodeBase.Tanks_Turrets.Services.Control.Enemies;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.Data;
using CodeBase.Tanks_Turrets.Services.Control.Enemies.States;
using CodeBase.Tanks_Turrets.Services.Control.Player;
using CodeBase.Tanks_Turrets.Services.Factory;
using Cysharp.Threading.Tasks;

namespace CodeBase.GameRule.GameStates._GameplayInitState
{
    public sealed class GameplayInitState : IGameState, IDisposable
    {
        private readonly LevelMap _levelMap;
        private readonly MapNavigator _mapNavigator;
        private readonly OnPathTankMover _onPathTankMover;
        private readonly TankAndTurretsFactory _tankAndTurretsFactory;

        private GameConfiguration _gameConfiguration;

        private readonly PlayerContainer _playerContainer = new ();
        private readonly List< EnemyContainer> _enemyContainer = new ();

        public Tank Player => _playerContainer.Tank;
        public IEnumerable<Tank> EnemiesTanks => _enemyContainer.Select(e => e.Tank);
        public IEnumerable<Turret> EnemiesTurrets => _enemyContainer.Select(e => e.Turret);

        public GameplayInitState(PlayerInputHandler playerInput,
            TankAndTurretsFactory tankAndTurretsFactory,
            LevelMap levelMap, MapNavigator mapNavigator, OnPathTankMover onPathTankMover)
        {
            _playerContainer.PlayerInput = playerInput;
            _tankAndTurretsFactory = tankAndTurretsFactory;
            _levelMap = levelMap;
            _mapNavigator = mapNavigator;
            _onPathTankMover = onPathTankMover;
        }

        public void FetchConfiguration(GameConfiguration configuration)
            => _gameConfiguration = configuration;

        public void Enter()
        {
            _levelMap.CreateMap();
            InitPlayer();
            InitEnemies();
            RunAll();
        }

        private void InitPlayer()
        {
            _playerContainer.Tank = _tankAndTurretsFactory.CreatePlayerTank();
            _playerContainer.TankSteering = new TankSteering(_playerContainer.Tank);
            _playerContainer.PlayerInput.Init(_playerContainer.TankSteering);
            BindCamera().Forget();
        }

        private void InitEnemies()
        {
            var iterations = _gameConfiguration.EnemyTankCount + _gameConfiguration.EnemyTurretCount;
            var tankCount = 0;
            var turretCount = 0;
            for (var i = 0; i < iterations; i++)
            {
                CombatTowerMachine enemyMachine = null;
                var enemy = new EnemyContainer();
                if (tankCount < _gameConfiguration.EnemyTankCount)
                {
                    enemyMachine = _tankAndTurretsFactory.CreateEnemyTank();
                    enemy.Tank = (Tank)enemyMachine;
                    tankCount++;
                }
                else
                {
                    enemyMachine = _tankAndTurretsFactory.CreateEnemyTurret();
                    enemy.Turret = (Turret)enemyMachine;
                    turretCount++;
                }
                
                var steeringSettings = 
                    new MachineControlSettings(_mapNavigator, _levelMap, enemyMachine, _playerContainer.Tank);
                enemy.Steering = new AISteering(_onPathTankMover, steeringSettings);
                enemy.DestroyedActions = () => OnEnemyDestroyed(enemy);
                enemyMachine.Destroyed += enemy.DestroyedActions;
                _enemyContainer.Add(enemy);
            }
        }
        
        private void RunAll()
        {
            _playerContainer.Tank.transform.position = _levelMap.PlayerSpawnPoint;
            _playerContainer.Tank.gameObject.SetActive(true);

            var spawnIterator = 0; 
            _enemyContainer.ForEach(e =>
            {
                CombatTowerMachine machine = null;
                machine = e.Tank is not null ? e.Tank : e.Turret;

                machine.transform.position = _levelMap.EnemySpawnPoints.ElementAt(spawnIterator++);
                machine.gameObject.SetActive(true);
                
                if (e.Tank is not null) 
                    e.Steering.EnterStare<TankPatrolState>();
                else 
                    e.Steering.EnterStare<MachineChaseState>();
            });
        }

        private async UniTaskVoid BindCamera()
        {
            while (true)
            {
                await UniTask.Yield();
                if(Camera is null) continue;
                
                var transform = _playerContainer.Tank.transform;
                Camera.Follow = transform;
                Camera.LookAt = transform;
                return;
            }
        }

        private void OnEnemyDestroyed(EnemyContainer enemy)
            => _enemyContainer.Remove(enemy);

        private ICinemachineCamera Camera =>
            UnityEngine.Camera.main!.gameObject.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        public void Dispose()
        {
            _enemyContainer.ForEach(e =>
            {
                CombatTowerMachine machine = e.Tank is not null ? e.Tank : e.Turret;
                machine.Destroyed -= e.DestroyedActions;
            });
        }
    }
}