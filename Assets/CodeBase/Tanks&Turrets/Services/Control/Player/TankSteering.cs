using CodeBase.Tanks_Turrets.Data.Entities;
using UnityEngine;

namespace CodeBase.Tanks_Turrets.Services.Control.Player
{
    public sealed class TankSteering
    {
        public Tank TargetTank { get; }

        public TankSteering(Tank tank)
        {
            TargetTank = tank;
        }
        
        public void Move(int value)
        {
            var transform = TargetTank.transform;
            transform.position += transform.right * value * MoveSpeed;
        }

        public void Turn(int value)
        {
            var rotation = TargetTank.transform.rotation.eulerAngles;
            rotation.z += value * TankRotationSpeed;
            TargetTank.transform.rotation = Quaternion.Euler(rotation);
        }
        
        public void RotateTower(Vector3 look)
        {
            var transform = TargetTank.Tower;
            look = new Vector3(look.x, look.y, transform.position.z);

            var direction = new Vector2(
                look.x - transform.position.x,
                look.y - transform.position.y
            );
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TowerRotationSpeed);
            
        }

        private float MoveSpeed => TargetTank.TankConfiguration.Motion.MoveSpeed * Time.deltaTime;
        private float TankRotationSpeed => TargetTank.TankConfiguration.Motion.RotationSpeed * Time.deltaTime;
        private float TowerRotationSpeed => TargetTank.TankConfiguration.CombatReadiness.TowerRotationSpeed * Time.deltaTime;
    }
}