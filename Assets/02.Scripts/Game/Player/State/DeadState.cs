using UnityEngine;

public class DeadState : IPlayerState
{
    [SerializeField] private EventManager eventManager;

    public void Dead()
    {
        eventManager.RequestGameOver();
    }
}
