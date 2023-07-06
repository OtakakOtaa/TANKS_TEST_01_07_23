namespace CodeBase.CombatReadinessSystems.Behaviors
{
    public interface ICanFire
    {
        float AttackAmount { get; set; }
        void Fire();
    }
}