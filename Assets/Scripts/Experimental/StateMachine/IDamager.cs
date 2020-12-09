public interface IDamager
{
    int Damage { get; }
    DamageType TypeOfDamage { get; }
}

public enum DamageType { Electrical, Fire, Water, Earth };