using System.Threading;

namespace CodeBase.Tanks_Turrets.Services.Control.Enemies.Data
{
    public interface IEnemyMachineState
    {
        void Enter(CancellationToken exitToken);
        void OnCanSeePlayer();
    }
}