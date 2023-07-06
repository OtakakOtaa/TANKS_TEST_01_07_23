
using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Services._FireSystem;

namespace CodeBase.Tanks_Turrets.Data.Entities
{
    public sealed class Turret : CombatTowerMachine
    {
        public TurretConfiguration TurretConfiguration { get; private set; }

        public void Constructor(FireSystem fireSystem, TurretConfiguration configuration)
        {
            TurretConfiguration = configuration;
            base.Constructor(fireSystem, configuration.CombatReadinessConfiguration);
        }
    }
}