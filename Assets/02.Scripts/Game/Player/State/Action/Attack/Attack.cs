using UnityEngine;

public class Attack
{
    // Bow 사용 시 Upper Body가 안 움직일 것 같은데 수정해야겠음
    [SerializeField] PlayerController con;

    public Attack(PlayerController controller)
    {
        con = controller;
    }
    public void SwordAttack(AttackState.AttackStyle attackStyle)
    {
        switch (attackStyle)
        {
            case AttackState.AttackStyle.Light:
                {
                    break;
                }
            case AttackState.AttackStyle.Heavy:
                {
                    break;
                }
        }
        con.Player.ChangeAttackType(Player.AttackType.Sword);
    }

    public void BowAttack()
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
        con.Player.ChangeAttackType(Player.AttackType.Bow);

    }
    public void BombAttack()
    {
        con.Player.ChangeAttackType(Player.AttackType.Bomb);
    }

}
