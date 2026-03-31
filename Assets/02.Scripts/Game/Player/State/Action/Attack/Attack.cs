using UnityEngine;

public class Attack
{
    [SerializeField] PlayerController controller;
    [SerializeField] AttackState attackState;
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
        controller.Player.ChangeAttackType(Player.AttackType.Sword);
    }

    public void BowAttack()
    {
        if (controller.Input.BowPressed) { }
        if (controller.Input.BowReleased) 
        {
            controller.Player.ChangeAttackType(Player.AttackType.Bow);
        }
    }
    public void BombAttack()
    {
        controller.Player.ChangeAttackType(Player.AttackType.Bomb);
    }

}
