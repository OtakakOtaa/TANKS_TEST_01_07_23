
namespace CodeBase.CombatReadinessSystems.Behaviors
{
    public interface IHitable
    {
        float HeathAmount { get; set; }
        float MaxHeathAmount { get; set; }

        void Hit(float damageAmount)
        {
            HeathAmount -= damageAmount;
            var isDie = HeathAmount is 0 or < 0;
            if (!isDie) return;
            HeathAmount = 0;
        }
        
        bool IsDie => HeathAmount is 0;
    }
}