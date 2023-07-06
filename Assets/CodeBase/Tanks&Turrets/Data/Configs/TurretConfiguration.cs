using UnityEngine;

namespace CodeBase.Tanks_Turrets.Data.Configs
{
    [CreateAssetMenu(menuName = "Configurations/Machines/" + nameof(TurretConfiguration), order = default)]
    public sealed class TurretConfiguration : ScriptableObject
    {
        [SerializeField] private CombatReadinessConfiguration _combatReadinessConfiguration;

        public CombatReadinessConfiguration CombatReadinessConfiguration => _combatReadinessConfiguration;
    }
}