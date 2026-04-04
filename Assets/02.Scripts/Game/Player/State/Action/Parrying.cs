using UnityEngine;

public class Parrying : MonoBehaviour
{
    [SerializeField] private float InvincibleTime = 0.3f;
    private PlayerController con;
    
    private float timer;
    public Parrying(PlayerController controller)
    {
        con = controller;
    }    
    
    public void Enter()
    {
        con.Animation.SetParry(true);
        timer = InvincibleTime;
        con.ActionState.ChangeInvincible(true);
    }

    public void Exit()
    {
        con.Animation.SetParry(false);
    }
    private void Update()
    {
        if(!con.Input.ParryingPressed && con.ActionState.currentType == ActionState.ActionType.Parrying)
        {
            // Parrying 타이밍에 따라 들어가는 데미지 분리는 나중에 짜기로
            con.ActionState.ChangeType(ActionState.ActionType.Idle);
        }

        if (con.ActionState.currentType != ActionState.ActionType.Parrying)
            return;

        // if(con.ActionState.currentType == ActionState.ActionType.Parrying) -> 나중에 코드 추가
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            con.ActionState.ChangeInvincible(false);
        }

    }
}
