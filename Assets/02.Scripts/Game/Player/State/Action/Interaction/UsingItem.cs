using System.Collections;
using UnityEngine;
using static PlayerStateMachine;

public class UsingItem : PlayerBehaviour
{
    // Item
    // 이건 무기를 들고 있을때도 생각해 봐야될듯
    [SerializeField] private Renderer Potion;
    [SerializeField] private Renderer Bomb;
    
    private Coroutine enterCoroutine;
    public void Enter()
    {
        if (enterCoroutine == null)
        {
            // 여기서 Item 수 차감
            // 근데 아직 구현 안 해서 나중에 할듯
            enterCoroutine = StartCoroutine(EnterItem());
        }
    }

    public void Exit()
    {
        con.InteractionState.TryChangeInteractionType(InteractionState.InteractionType.Idle);
        // 이름 확인 
        con.StateMachine.TryChangeState(PlayerState.LocomotionState);
    }

    IEnumerator EnterItem()
    {
        float time = 0;
        while (time >= 1)
        {
            time += Time.deltaTime * 3;
            con.Animation.SetLayerWeight(4, time);
            yield return null;
        }
    }
}
