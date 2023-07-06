using CodeBase.Map.Services;
using CodeBase.Map.Services.Core;
using CodeBase.Tanks_Turrets.Data.Entities;

namespace CodeBase.Tanks_Turrets.Services.Control.Enemies.Data
{
    public sealed class MachineControlSettings
    {
        public MapNavigator MapNavigator { get; }
        public LevelMap LevelMap { get; }
        public CombatTowerMachine Enemy { get; }
        public Tank Player { get; }

        public MachineControlSettings(MapNavigator mapNavigator, LevelMap levelMap, 
            CombatTowerMachine enemy, Tank player)
        {
            MapNavigator = mapNavigator;
            LevelMap = levelMap;
            Enemy = enemy;
            Player = player;
        }
    }
}