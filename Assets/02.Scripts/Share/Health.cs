using UnityEngine;


public class Health : IDamageable
{
    private PlayerController con;
    public float HP { get; private set; }

    public bool isDead { get; private set; }
    public Health(PlayerController controller)
    {
        con = controller;
    }

    public void GetHPPosion(float value)
    {
        HP += value;
    }
    public void PlayerGetDamage(float value)
    {
        if (con.Player.IsInvincible && con.ActionState.currentType == ActionState.ActionType.Dodge)
            return;
        else if (con.Player.IsInvincible && con.ActionState.currentType == ActionState.ActionType.Parrying)
            return;
        else if (!con.Player.IsInvincible && con.ActionState.currentType == ActionState.ActionType.Parrying)
            HP -= value / 2;
        else
            HP -= value;

        if (HP <= 0)
        {
            con.StateMachine.ChangePlayerState(PlayerStateMachine.PlayerState.DeadState);
        }
    }
    public void MonsterGetDamage(float value)
    {
        HP -= value;
    }

}
