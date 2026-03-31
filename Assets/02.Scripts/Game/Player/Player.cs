using UnityEngine;

public class Player : IDamageable
{
    // 매 Attack마다 ChangeAttackType를 해줘야
    // 무기 바꿀 때 바꾼지 확인 가능
    public AttackType currentAttackType { get; private set; }
    public AttackType previousAttackType { get; private set; }
    public enum AttackType
    {
        Sword,
        Bow,
        Bomb
    }
    public void ChangeAttackType(AttackType type)
    {
        if (currentAttackType == type) return;

        previousAttackType = currentAttackType;
        currentAttackType = type;
    }

    public ItemType CurrentItemType {  get; private set; }
        public enum ItemType
    {
        HPPosion,
        Bomb
    }

}
