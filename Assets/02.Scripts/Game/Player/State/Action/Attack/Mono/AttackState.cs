using UnityEngine;

public class AttackState : PlayerBehaviour
{
    public AttackStyle currentAttackStyle { get; private set; }
    public enum AttackStyle 
    {
        Light,
        Heavy
    }
    public void ChangeAttackStyle(AttackStyle style)
    {
        if (currentAttackStyle == style) return;
        currentAttackStyle = style;
    }
    private void Update()
    {
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;
        if (con.Input.LightAttack)
        {
            ChangeAttackStyle(AttackStyle.Light);
            con.Animation.SetAttackType(0);
        }

        if (con.Input.HeavyAttack)
        {
            ChangeAttackStyle(AttackStyle.Heavy);
            con.Animation.SetAttackType(1);
        }
    }
    private void FixedUpdate()
    {
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;

        switch (con.Player.currentAttackType)
        {
            case Player.AttackType.Sword:
                con.Animation.SetWeaponType(2);
                con.Animation.PlayAttack();
                con.Attack.SwordAttack(currentAttackStyle);
                break;
            case Player.AttackType.Bow:
                con.Animation.SetWeaponType(1);
                con.Attack.BowAttack();
                break;
            case Player.AttackType.Bomb:
                con.Attack.BombAttack();
                break;
        }
    }
}
