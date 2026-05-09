using System.Collections;
using UnityEngine;
using static AttackState;

public class Attack : PlayerBehaviour
{
    private Coroutine coroutine;
    // Bow 사용 시 Upper Body가 안 움직일 것 같은데 수정해야겠음
    // 공격시 모든 움직임 멈춰야함
    // 넉벡 공격에 넉벡 당함 넉벡이 우선임
    // 
    public Attack(PlayerController controller)
    {
        con = controller;
    }
    public void RequestSwordAttack()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(SwordAttack());
        }
    }

    IEnumerator SwordAttack()
    {
        switch (con.AttackState.currentAttackStyle)
        {
            case AttackState.AttackStyle.Light:
                while (true)
                {
                    // hitEnemies 클리어 코드

                    lightAttackProcedure();

                    // time * 0.2초동안 대기
                    // 무기 콜라이더 활성화 코드 -> Sword 코드는 나중에 따로 작성
                    // time * 0.8초동안 대기
                    // 무기 콜라이더 비활성화 코드
                    
                    yield return new WaitUntil(() => !con.AttackState.isAttacking);

                    if (!con.Input.AttackPressed)
                        break;
                }
                break;
            case AttackState.AttackStyle.Heavy:
                con.AttackState.ChangeAttackStyle(AttackStyle.Heavy);
                con.Animation.SetAttackType(1);
                yield return new WaitUntil(() => !con.AttackState.isAttacking);
                break;
        }
        coroutine = null;
        con.AttackState.ChangeAttackStyle(AttackStyle.Default);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);
    }

    public void RequestBowAttack()
    {
        if (coroutine == null)
        {
            coroutine = StartCoroutine(BowAttack());
        }
    }
    IEnumerator BowAttack()
    {
        if (con.Input.StartBowCharging)
        {
            con.Animation.SetBowAim(true);
        }
        if (con.Input.BowCharging)
        {
        }
        if (con.Input.BowShoot)
        {
            con.Animation.SetBowAim(false);
        }
        con.Player.ChangeWeaponType(Player.WeaponType.Bow);

        coroutine = null;

        yield return null;
    }
    private void lightAttackProcedure()
    {
        con.Animation.SetAttackType(0);

        con.AttackState.StartAttacking();
        con.AttackState.ChangeAttackStyle(AttackStyle.Light);

        con.Input.AckAttack();
    }
}
