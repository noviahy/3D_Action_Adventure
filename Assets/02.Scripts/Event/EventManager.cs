using UnityEngine;
using System;

public class EventManager
{
    public event Action OnGameOver;
    public event Action OnGameStart;
    public event Action OnInteractionClick;

    private PlayerController con;
    public EventManager(PlayerController playerController)
    {
        con = playerController;
    }
    public void RequestGameOver()
    {
        OnGameOver?.Invoke();
    }
    public void RequestGameStart()
    {
        OnGameStart?.Invoke();
    }
    public void RequestInteraction()
    {
        OnInteractionClick?.Invoke();
    }
}
