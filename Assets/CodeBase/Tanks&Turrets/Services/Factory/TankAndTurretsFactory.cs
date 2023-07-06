using CodeBase.Configurations;
using CodeBase.Tanks_Turrets.Data;
using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Data.Entities;
using CodeBase.Tanks_Turrets.Services._FireSystem;
using UnityEngine;
using VContainer;

namespace CodeBase.Tanks_Turrets.Services.Factory
{
    public sealed class TankAndTurretsFactory
    {
        private readonly GameConfiguration _configuration;
        private readonly IObjectResolver _resolver;

        public TankAndTurretsFactory(IObjectResolver resolver, GameConfiguration gameConfigurations)
        {
            _resolver = resolver;
            _configuration = gameConfigurations;
        }

        public Tank CreatePlayerTank()
            => CreateTank(_configuration.PlayerTankConf, _configuration.PlayerTank);

        public Tank CreateEnemyTank()
            => CreateTank(_configuration.EnemyTankConf, _configuration.EnemyTank);

        public Turret CreateEnemyTurret()
        {
            var turret = Object.Instantiate(_configuration.EnemyTurret);
            turret.Constructor(_resolver.Resolve<FireSystem>(), _configuration.EnemyTurretConf);
            turret.gameObject.SetActive(false);
            return turret;
        }
        

        private Tank CreateTank(TankConfiguration config, Tank prefab)
        {
            var tank = Object.Instantiate(prefab);
            tank.Constructor(_resolver.Resolve<FireSystem>(), config);
            tank.gameObject.SetActive(false);
            return tank;
        }
    }
}