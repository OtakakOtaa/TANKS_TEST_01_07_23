using CodeBase.Map.Obstacles;
using CodeBase.Map.Services.Core;
using CodeBase.Tanks_Turrets.Data;
using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Data.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace CodeBase.Configurations
{
    [CreateAssetMenu(menuName = "Configurations/Machines/" + nameof(GameConfiguration), order = default)]
    public class GameConfiguration : ScriptableObject
    {
        [Header("--//--Tanks&Turrets--//--")]
        
        [Header("Player")]
        [SerializeField] private TankConfiguration _playerTankConf;
        [SerializeField] private Tank _playerTank;
        
        [Header("Enemy Tank")]
        [SerializeField] private TankConfiguration _enemyTankConf;
        [SerializeField] private Tank _enemyTank;
        
        [FormerlySerializedAs("_enemyTurretConfig")]
        [Header("Enemy Turret")]
        [SerializeField] private TurretConfiguration _enemyTurretConf;
        [SerializeField] private Turret _enemyTurret;
        
        [Header("Map")] 
        [SerializeField] private LevelMap.Configuration _mapConfiguration;
        [SerializeField] private ObstacleBox _obstacleBox;
        [SerializeField] private int _boxCount;
        [SerializeField] private int _boxHeath;

        [Header("Level Prompts")] 
        [SerializeField, Range(1, 10)] private int _enemyTankCount;
        [SerializeField, Range(1, 10)] private int _enemyTurretCount;
        

        public TankConfiguration PlayerTankConf => _playerTankConf;
        
        public TankConfiguration EnemyTankConf => _enemyTankConf;

        public TurretConfiguration EnemyTurretConf => _enemyTurretConf;
        
        public Tank PlayerTank => _playerTank;

        public Tank EnemyTank => _enemyTank;

        public LevelMap.Configuration MapConfiguration => _mapConfiguration;


        public Turret EnemyTurret => _enemyTurret;

        public int EnemyTankCount => _enemyTankCount;

        public int EnemyTurretCount => _enemyTurretCount;

        public int BoxCount => _boxCount;

        public int BoxHeath => _boxHeath;

        public ObstacleBox ObstacleBox => _obstacleBox;
    }
}