using System;
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
    public AttackState(PlayerController controller)
    {
        con = controller;
    }
    public void Enter()
    {
        con.Animation.SetLayerWeight(0, 0);
        con.Animation.SetLayerWeight(1, 1);
        con.Animation.SetLayerWeight(2, 0);
    }
    private void Update() // 에니메이션 관리
    {
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;
        switch (con.Player.currentWeaponType)
        {
            case Player.WeaponType.Sword:
                con.Attack.RequestSwordAttack();
                break;
            case Player.WeaponType.Bow:
                con.Attack.RequestBowAttack();
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
        con.Animation.SetLayerWeight(0, 0);
        con.Animation.SetLayerWeight(1, 1);
        con.Animation.SetLayerWeight(2, 0);
    }
}
