using CodeBase.Tanks_Turrets.Data.Entities;
using CodeBase.Tanks_Turrets.Services.Control.Player;

namespace CodeBase.GameRule.GameStates._GameplayInitState
{
    public sealed class PlayerContainer
    {
        public PlayerInputHandler PlayerInput;
        public TankSteering TankSteering;
        public Tank Tank;
    }
}