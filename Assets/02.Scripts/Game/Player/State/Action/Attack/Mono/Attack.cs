using System.Collections;
using UnityEngine;
using static AttackState;

public class Attack : PlayerBehaviour
{
    public bool BowAimed { get; private set; }
    public bool BowShoot { get; private set; }
    public bool Standby { get; private set; }

    private Coroutine coroutine;
    private Coroutine exitCoroutine;
    private Coroutine enterCoroutine; // 안 쓰긴 하는데 나중에 필요할까봐 세트로 만들어둠
    private Coroutine releaseCoroutine;
    private int combo = 1;
    // 넉벡 공격에 넉벡 당함 넉벡이 우선임
    public Attack(PlayerController controller)
    {
        con = controller;
    }
    private void Start()
    {
        currentBowState = bowState.Idle;
        Standby = false;
        BowShoot = true;
    }
    public void RequestSwordAttack()
    {
        if (coroutine == null)
            coroutine = StartCoroutine(SwordAttack());
    }

    IEnumerator SwordAttack()
    {
        // Debug.Log("!");
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
                    yield return new WaitUntil(() => !con.AttackState.isAttacking);

                    if (!con.Input.AttackPressed)
                        break;
                    if (con.Input.HeavyAttack)
                        break;
                    combo++;
                    if (combo > 3)
                        combo = 1;
                }
                break;
            case AttackState.AttackStyle.Heavy:
                HeavyAttackProcedure();
                yield return new WaitUntil(() => !con.AttackState.isAttacking);
                break;
        }
        con.AttackState.ChangeAttackStyle(AttackStyle.Default);
        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.Animation.PlayUpperBody("SwordIdle");
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);
        coroutine = null;
    }
    private void lightAttackProcedure()
    {
        con.Animation.PlayLightAttack(combo);

        con.AttackState.StartAttacking();

        con.Input.AckAttack();
    }
    private void HeavyAttackProcedure()
    {
        con.Animation.PlayHeavyAttack();

        con.AttackState.StartAttacking();

        con.Input.AckAttack();
    }

    // Bow
    public enum bowState
    {
        Idle,
        Enter,
        Released,
        Exiting
    }
    public bowState currentBowState { get; private set; }
    private bowState preBowState;
    public void RequestBowAttack() // 외부에서 호출하는 유일한 함수입니다.
    {
        if (currentBowState == bowState.Idle) // 외부에선 꼬임 방지를 위해 Idle에서만 상태를 변경할 수 있게 했어요
            ChangeBowState(bowState.Enter);
    }

    private void ChangeBowState(bowState state)
    {
        preBowState = currentBowState;
        currentBowState = state;

        switch (state)
        {
            case bowState.Idle:
                break;
            case bowState.Enter:
                enterCoroutine = StartCoroutine(BowEnter());
                break;
            case bowState.Released:
                if (releaseCoroutine != null)
                    StopCoroutine(releaseCoroutine);
                if (exitCoroutine != null)
                {
                    StopCoroutine(exitCoroutine);
                    exitCoroutine = null;
                    con.Animation.SetLayerWeight(3, 1);
                }
                releaseCoroutine = StartCoroutine(BowRelease());
                break;
            case bowState.Exiting:
                exitCoroutine = StartCoroutine(BowExit());
                break;
        }
    }

    IEnumerator BowEnter()
    {
        BowAimed = true;
        BowShoot = false;
        con.Animation.PlayLoadBow();

        if (con.Input.BowCharging)
        {
            // 레이어 활성화
            float t = 0;

            while (t <= 1)
            {
                t += Time.deltaTime * 4;
                con.Animation.SetLayerWeight(3, t);

                yield return null;
            }
            con.Animation.SetLayerWeight(3, 1);
            enterCoroutine = null;

        }

        ChangeBowState(bowState.Released);
    }
    IEnumerator BowRelease()
    {
        if (preBowState != bowState.Enter)
        {
            con.Animation.PlayLoadBow();
            BowAimed = true;
            BowShoot = false;
        }

        while (con.Input.BowCharging)
            yield return null;

        con.Animation.PlayUpperBody("Release");
        con.Animation.PlayLowerBody("BowIdle");
        float t = 0;

        // 활 쏘는 모션까지 좀 기다려야 rig가 쏘는 애니메이션에도 활성화된 상태일 수 있는 www
        while(t <= 0.4f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        BowShoot = true;

        con.ActionState.TryChangeType(ActionState.ActionType.Idle);
        con.StateMachine.TryChangeState(PlayerStateMachine.PlayerState.LocomotionState);

        Standby = true;
        BowAimed = false;

        t = 0;
        while (t <= 1f)
        {
            t += Time.deltaTime;
            
            // 한번더!
            if (con.Input.BowCharging)
                ChangeBowState(bowState.Released);
            yield return null;
        }
        ChangeBowState(bowState.Exiting);
    }
    // 이건 왜 Idle로 안 뺐는가?
    // 그 이윤 Layer3번과 상호작용 하는게 없어서 그냥 여기서 0으로 가던 1으로 가던 상관 없음
    IEnumerator BowExit()
    {
        Standby = false;
        float t = 0;

        while (t <= 1)
        {
            if (con.Input.BowCharging)
            {
                ChangeBowState(bowState.Released);
                yield break;
            }
            t += Time.deltaTime * 2;
            con.Animation.SetLayerWeight(3, 1 - t);

            yield return null;
        }
        con.Animation.SetLayerWeight(3, 0);
        exitCoroutine = null;
        ChangeBowState(bowState.Idle);
    }

}
