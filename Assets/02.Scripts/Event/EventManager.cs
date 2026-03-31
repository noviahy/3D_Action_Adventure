using UnityEngine;
using System;

public class EventManager
{
    public static event Action OnGameOver;
    public static event Action OnGameStart;

    public void RequestGameOver()
    {
        OnGameOver?.Invoke();
    }
    public void RequestGameStart()
    {
        OnGameStart?.Invoke();
    }
}
