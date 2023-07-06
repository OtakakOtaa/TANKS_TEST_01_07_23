using System;
using CodeBase.Tanks_Turrets.Data.Entities;
using CodeBase.Tanks_Turrets.Services.Control.Enemies;

namespace CodeBase.GameRule.GameStates._GameplayInitState
{
    public sealed class EnemyContainer
    {
        public AISteering Steering;
        public Tank Tank;
        public Turret Turret;
        public Action DestroyedActions;
    }
}