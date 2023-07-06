using CodeBase.Tanks_Turrets.Data.Configs;
using CodeBase.Tanks_Turrets.Services._FireSystem;

namespace CodeBase.Tanks_Turrets.Data.Entities
{
    public sealed class Tank : CombatTowerMachine
    {
        public TankConfiguration TankConfiguration { get; private set; }

        public void Constructor(FireSystem fireSystem, TankConfiguration configuration)
        {
            TankConfiguration = configuration;
            base.Constructor(fireSystem, configuration.CombatReadiness);
        }
    }
}