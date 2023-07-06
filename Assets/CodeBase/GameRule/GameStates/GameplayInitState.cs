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
using UnityEngine;

namespace CodeBase.GameRule.GameStates
{
    public sealed class GameplayInitState : IGameState, IDisposable
    {
        private readonly PlayerInputHandler _playerInput;
        private readonly TankAndTurretsFactory _tankAndTurretsFactory;
        private readonly LevelMap _levelMap;
        private readonly MapNavigator _mapNavigator;
        private readonly OnPathTankMover _onPathTankMover;

        private GameConfiguration _gameConfiguration;

        private readonly List<AISteering> _tanksSteering = new();
        private readonly List<AISteering> _turretsSteering = new();
        private readonly List<Action> _tanksDestroyedActions = new();
        private readonly List<Action> _turretsDestroyedActions = new();

        public Tank Player { get; private set; }
        public List<Tank> EnemyTanks { get; } = new();
        public List<Turret> EnemyTurrets { get; } = new();

        public GameplayInitState(PlayerInputHandler playerInput,
            TankAndTurretsFactory tankAndTurretsFactory,
            LevelMap levelMap, MapNavigator mapNavigator, OnPathTankMover onPathTankMover)
        {
            _playerInput = playerInput;
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
            SpawnAll();
        }

        private void InitPlayer()
        {
            Player = _tankAndTurretsFactory.CreatePlayerTank();
            _playerInput.Init(new TankSteering(Player));
            BindCamera().Forget();
        }

        private void InitEnemies()
        {
            for (var i = 0; i < _gameConfiguration.EnemyTankCount; i++)
            {
                var tank = _tankAndTurretsFactory.CreateEnemyTank();
                var steeringSettings = new MachineControlSettings(_mapNavigator, _levelMap, tank, Player);
                _tanksSteering.Add(new AISteering(_onPathTankMover, steeringSettings));
                EnemyTanks.Add(tank);
                _tanksDestroyedActions.Add(() => OnEnemyTankDestroy(tank));
                tank.Destroyed += _tanksDestroyedActions[i];
            }
            
            for (var i = 0; i < _gameConfiguration.EnemyTurretCount; i++)
            {
                var turret = _tankAndTurretsFactory.CreateEnemyTurret();
                var steeringSettings = new MachineControlSettings(_mapNavigator, _levelMap, turret, Player);
                _turretsSteering.Add(new AISteering(_onPathTankMover, steeringSettings));
                EnemyTurrets.Add(turret);
                _turretsDestroyedActions.Add(() => OnTurretDestroy(turret));
                turret.Destroyed += _turretsDestroyedActions[i];
            }
        }
        
        private void SpawnAll()
        {
            Player.transform.position = _levelMap.PlayerSpawnPoint;
            Player.gameObject.SetActive(true);
            
            for (var i = 0; i < EnemyTanks.Count; i++)
            {
                EnemyTanks[i].transform.position = _levelMap.EnemySpawnPoints.ElementAt(i);
                EnemyTanks[i].gameObject.SetActive(true);
                _tanksSteering[i].EnterStare<TankPatrolState>();
            }
            for (var i = 0; i < EnemyTurrets.Count; i++)
            {
                EnemyTurrets[i].transform.position = _levelMap.EnemySpawnPoints.ElementAt(EnemyTanks.Count + i);
                EnemyTurrets[i].gameObject.SetActive(true);
                _turretsSteering[i].EnterStare<MachineChaseState>();
            }
        }

        private async UniTaskVoid BindCamera()
        {
            ICinemachineCamera camera = null;
            while (camera is null)
            {
                camera = GetCamera; 
                
                await UniTask.Yield();
                if(camera is null) continue; 
                    
                var transform = Player.transform;
                GetCamera.Follow = transform;
                GetCamera.LookAt = transform;    
            }
        }

        private ICinemachineCamera GetCamera =>
            Camera.main!.gameObject.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

        private void OnEnemyTankDestroy(Tank tank)
        {
            _tanksSteering.RemoveAt(EnemyTanks.IndexOf(tank));
            EnemyTanks.Remove(tank);
        }

        private void OnTurretDestroy(Turret turret)
        {
            _turretsSteering.RemoveAt(EnemyTurrets.IndexOf(turret));
            EnemyTurrets.Remove(turret);
        }

        public void Dispose()
        {
            for (var i = 0; i < _tanksDestroyedActions.Count; i++)
                EnemyTanks[i].Destroyed -= _tanksDestroyedActions[i];
            for (var i = 0; i < _turretsDestroyedActions.Count; i++) 
                EnemyTurrets[i].Destroyed -= _turretsDestroyedActions[i];
        }
    }
}