using CodeBase.CombatReadinessSystems.Behaviors;
using CodeBase.Infrastructure.OneEntity;
using CodeBase.Tanks_Turrets.Visual;
using UnityEngine;

namespace CodeBase.Map.Obstacles
{
    public sealed class ObstacleBox : GameEntity, IHitable
    {
        [SerializeField] private HeathBar _heathBar;

        public float HeathAmount { get; set; }
        public float MaxHeathAmount { get; set; }

        public void Constructor(float heath)
        {
            HeathAmount = heath;
            MaxHeathAmount = heath;
            _heathBar.Init(this);
        }
    }
}