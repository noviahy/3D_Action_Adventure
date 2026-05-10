using System;
using UnityEngine;

public class AttackState : PlayerBehaviour
{
    public AttackStyle currentAttackStyle { get; private set; }
    public bool isAttacking { get; private set; }

    private void Start()
    {
        currentAttackStyle = AttackStyle.Default;
    }
    public enum AttackStyle 
    {
        Default,
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
        // Debug.Log($"AttackStyle:{currentAttackStyle}");
        Debug.Log($"ActionState:{con.ActionState.currentType}");
        Debug.Log($"PlayerState:{con.StateMachine.currentState}");
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;
        switch (con.Player.currentWeaponType)
        {
            case Player.WeaponType.Sword:
                if (con.Input.LightAttack)
                    ChangeAttackStyle(AttackStyle.Light);
                if(con.Input.HeavyAttack)
                    ChangeAttackStyle(AttackStyle.Heavy);
                con.Attack.RequestSwordAttack();
                break;
            case Player.WeaponType.Bow:
                con.Attack.RequestBowAttack();
                break;
        }
    }

    public void Exit()
    {
        con.Animation.SetLayerWeight(0, 0);
        con.Animation.SetLayerWeight(1, 0);
        con.Animation.SetLayerWeight(2, 1);
        
    }
    public void StartAttacking()
    {
        isAttacking = true;
    }
    public void FinishAttacking()
    {
        isAttacking = false;
    }

}
