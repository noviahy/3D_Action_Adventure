using System.Collections;
using UnityEngine;

public class Attack : PlayerBehaviour
{
    [SerializeField] private Collider SwordCollider;

    private Coroutine coroutine;
    private int comboIndex = 0;
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
            coroutine = StartCoroutine(SwordAttack());
    }

    IEnumerator SwordAttack()
    {
        yield return new WaitUntil(() => !con.Input.isAttacking);

        switch (con.AttackState.currentAttackStyle)
        {
            case AttackState.AttackStyle.Light:
                {
                    con.Animation.PlayAttack();
                    con.Input.StartAttacking();
                    if (con.Input.isAttacking)
                    {
                        comboIndex++;

                        if (comboIndex > 2)
                            comboIndex = 0;
                        
                        con.Input.AckAttack();
                    }
                    break;
                }
            case AttackState.AttackStyle.Heavy:
                {
                    break;
                }
        }

        coroutine = null;
        yield return null;
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
            con.Animation.PlayAttack();
        }
        con.Player.ChangeWeaponType(Player.WeaponType.Bow);

        coroutine = null;

        yield return null;
    }
    public void EnableCombo()
    {
        SwordCollider.enabled = false;
    }
    public void OnAttackEnd()
    {
        comboIndex = 0;
    }
    
}
