using UnityEngine;

public class AttackState : PlayerBehaviour
{
    public AttackStyle currentAttackStyle { get; private set; }

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
        /*
    switch (con.Player.currentWeaponType)
    {
         * 레이어값 변경이 있던 자리
         * 근데 Attack이 끝나자마자 Bow을 사용하니까 Layer가 안 꺼지는 문제가 있었음
         * 내가 봤을때 여기가 안 들어온 것 같은데 
         * 그래서 각자 코드 Enter로 넣어줌
    }
        */

    }
    private void Update()
    {
        // Debug.Log($"AttackStyle:{currentAttackStyle}");
        // Debug.Log($"ActionState:{con.ActionState.currentType}");
        // Debug.Log($"PlayerState:{con.StateMachine.currentState}");
        if (con.ActionState.currentType != ActionState.ActionType.Attack)
            return;
        //Debug.Log(con.Player.currentWeaponType);
        switch (con.Player.currentWeaponType)
        {
            case Player.WeaponType.Sword:
                if (con.Input.LightAttack)
                    ChangeAttackStyle(AttackStyle.Light);
                if (con.Input.HeavyAttack)
                    ChangeAttackStyle(AttackStyle.Heavy);
                con.Attack.RequestSwordAttack();
                break;
            case Player.WeaponType.Bow:
                con.BowAttack.RequestBowAttack();
                break;
        }
    }

    public void Exit()
    {
        // 있었던 코루틴을 빼줬습니다
        // 혹시 모르니 남겨두도록 할게요
    }
}
