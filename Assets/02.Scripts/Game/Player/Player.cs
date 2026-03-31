using UnityEngine;

public class Player : IDamageable
{
    public AttackType currentAttackType { get; private set; }
    public enum AttackType
    {
        Sword,
        Bow,
        Bomb
    }
    public void ChangeAttackType(AttackType type)
    {
        if (currentAttackType == type) return;
        currentAttackType = type;
    }

}
