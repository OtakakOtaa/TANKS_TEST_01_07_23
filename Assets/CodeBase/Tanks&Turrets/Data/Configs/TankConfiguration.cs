using System;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Data.Configs
{
    [CreateAssetMenu(menuName = "Configurations/Machines/" + nameof(TankConfiguration), order = default)]
    [Serializable] public sealed class TankConfiguration : ScriptableObject
    {
        [SerializeField] private MotionConfiguration _motionConfiguration;
        [SerializeField] private CombatReadinessConfiguration _combatReadinessConfiguration;

        public TankConfiguration(MotionConfiguration motionConfiguration,
            CombatReadinessConfiguration combatReadinessConfiguration)
        {
            _motionConfiguration = motionConfiguration;
            _combatReadinessConfiguration = combatReadinessConfiguration;
        }

        public MotionConfiguration Motion => _motionConfiguration;
        public CombatReadinessConfiguration CombatReadiness => _combatReadinessConfiguration;
    }
}