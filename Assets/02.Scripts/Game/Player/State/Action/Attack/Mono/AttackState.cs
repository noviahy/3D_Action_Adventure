using UnityEngine;
using static ActionState;

public class AttackState : PlayerBehaviour
{
    private PlayerController con;
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
    public AttackState(PlayerController controller)
    {
        con = controller;
    }
    public void Enter()
    {
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;

        switch (con.Player.currentWeaponType)
        {
            case Player.WeaponType.Sword:
                con.Animation.PlayAttack();
                con.Attack.SwordAttack(currentAttackStyle);
                break;
            case Player.WeaponType.Bow:
                con.Attack.BowAttack();
                break;
        }

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
    public void Exit()
    {

    }

    public void RequestColliderOnOff(bool value)
    {
        switch (con.Player.currentWeaponType)
        {
            case Player.WeaponType.Sword:
                con.Animation.SetWeaponType(2);
                con.Animation.PlayAttack();
                con.Attack.SwordAttack(currentAttackStyle);
                break;
            case Player.WeaponType.Bow:
                con.Animation.SetWeaponType(1);
                con.Attack.BowAttack();
                break;
        }
    }
}
