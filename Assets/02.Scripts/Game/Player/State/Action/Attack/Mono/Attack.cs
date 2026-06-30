using System.Collections;
using UnityEngine;
using static AttackState;


public class Attack : PlayerBehaviour
{
    private Coroutine coroutine;
    private int combo = 1;
    // 넉벡 공격에 넉벡 당함 넉벡이 우선임
    public Attack(PlayerController controller)
    {
        con = controller;
    }
    public void RequestSwordAttack()
    {
        if (coroutine == null)
            coroutine = StartCoroutine(SwordAttack());
    }

    IEnumerator SwordAttack()
    {
        switch (con.AttackState.currentAttackStyle)
        {
            case AttackState.AttackStyle.Light:
                combo = 1;

                while (true)
                {
                    // hitEnemies 클리어 코드

                    lightAttackProcedure();

                    // time * 0.2초동안 대기
                    // 무기 콜라이더 활성화 코드 -> Sword 코드는 나중에 따로 작성
                    // time * 0.8초동안 대기
                    // 무기 콜라이더 비활성화 코드
                    yield return new WaitForSeconds(1.16f);

                    if (!con.Input.AttackPressed)
                        break;
                    if (con.Input.HeavyAttack)
                        break;
                    combo++;
                    if (combo > 2)
                        combo = 1;
                }
                break;
            case AttackState.AttackStyle.Heavy:
                HeavyAttackProcedure();
                yield return new WaitForSeconds(1.8f);
                break;
        }
        coroutine = null;

        con.AttackState.ChangeAttackStyle(AttackStyle.Default);
        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);
    }
    private void lightAttackProcedure()
    {
        con.Animation.PlayLightAttack(combo);

        con.Input.AckAttack();
    }
    private void HeavyAttackProcedure()
    {
        con.Animation.PlayHeavyAttack();

        con.Input.AckAttack();
    }

}
