using CodeBase.Tanks_Turrets.Data.Configs;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services._FireSystem
{
    public interface IFireSystemSubscriber
    {
        void OnFire(CombatReadinessConfiguration configurations, Vector3 position, Transform muzzle);
        void OnHit(CombatReadinessConfiguration configurations, Vector3 position);
        void OnTankExploded(CombatReadinessConfiguration configurations, Vector3 position);
    }
}