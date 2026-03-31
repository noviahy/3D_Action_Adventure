using UnityEngine;

public class Health
{
    [SerializeField] PlayerStateMachine playerStateMachine;
    public float HP;

    public bool isDead { get; private set; }

    public void GetHPPosion(float value)
    {
        HP += value;
    }
    public void GetDamage(float value)
    {
        HP -= value;
        if(HP <= 0 && playerStateMachine != null)
        {
            playerStateMachine.ChangePlayerState(PlayerStateMachine.PlayerState.DeadState);
        }
    }

}
