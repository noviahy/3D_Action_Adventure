using System.Collections;
using UnityEngine;

public class ActionIdle : PlayerBehaviour
{
    public bool IdleBlending { get; private set; } = false;
    public void Enter(ActionState.ActionType preState)
    {
        switch (preState)
        {
            case ActionState.ActionType.Dodge:
                IdleBlending = true;
                StartCoroutine(OffLayer1());
                break;
            case ActionState.ActionType.Attack:
                IdleBlending = true;
                if (con.Player.currentWeaponType == Player.WeaponType.Sword)
                    StartCoroutine(OnLayer2());
                break;
        }
    }
    IEnumerator OnLayer2() // ·¹ÀÌ¾î 1 ²ô°í 2 ÄÔ
    {
        // Layer1: 1 -> 0
        // Layer2: 0 -> 1
        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * 6;

            con.Animation.SetLayerWeight(2, t);
            yield return null;
        }
        con.Animation.SetLayerWeight(2, 1);
        StartCoroutine(OffLayer1());
    }
    IEnumerator OffLayer1() // ·¹ÀÌ¾î 1 ²û
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 1.5f;

            con.Animation.SetLayerWeight(1, 1 - t);
            yield return null;
        }

        IdleBlending = false;
        yield return null;

    }
    IEnumerator OffLayer3()
    {
        float t = 0;
        while (t <= 1)
        {
            t += Time.deltaTime * 1.5f;

            con.Animation.SetLayerWeight(1, 1 - t);
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
}
