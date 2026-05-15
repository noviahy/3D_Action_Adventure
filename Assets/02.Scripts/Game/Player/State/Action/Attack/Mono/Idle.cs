using System.Collections;
using UnityEngine;

public class Idle : PlayerBehaviour
{
    public bool IdleBlending { get; private set; } = false;
    private Coroutine layer1Coroutine;
    private Coroutine dodgeCoroutine;
    public void Enter(ActionState.ActionType preState)
    {
        switch (preState)
        {
            case ActionState.ActionType.Dodge:
                IdleBlending = true;
                dodgeCoroutine = StartCoroutine(FromDodge());
                break;
            case ActionState.ActionType.Attack:
                IdleBlending = true;
                if (con.Player.currentWeaponType == Player.WeaponType.Sword)
                    StartCoroutine(FromAttackLayer2());
                break;
        }
    }
    IEnumerator FromAttackLayer2()
    {
        // Layer1: 1 -> 0
        // Layer2: 0 -> 1
        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * 6;

            con.Animator.SetLayerWeight(2, t);
            yield return null;
        }
        con.Animation.SetLayerWeight(2, 1);

        layer1Coroutine = StartCoroutine(FromAttackLayer1());
    }
    IEnumerator FromAttackLayer1()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 2;

            con.Animator.SetLayerWeight(1, 1 - t);
            yield return null;
        }

        IdleBlending = false;
        yield return null;

    }
    IEnumerator FromDodge()
    {
        // Layer1: 1 -> 0
        float w = 0;
        while (w < 1f)
        {
            w += Time.deltaTime * 1.5f;
            con.Animation.SetLayerWeight(1, 1 - w);
            yield return null;
        }

        IdleBlending = false;
        yield return null;
    }
    public void RequestStopAllCoroutine()
    {
        StopAllCoroutines();
        IdleBlending = false;
    }
    public void RequestStopLayer1()
    {
        if (layer1Coroutine != null)
            StopCoroutine(layer1Coroutine);
        IdleBlending = false;
    }
    public void RequestStopDodgeLayer()
    {
        if (dodgeCoroutine != null)
            StopCoroutine(dodgeCoroutine);
        IdleBlending = false;
    }
}
