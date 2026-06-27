using UnityEngine;
using System.Collections;

public class InteractionIdle : PlayerBehaviour
{
    public bool IdleBlending { get; private set; } = false;
    private Coroutine layer1Coroutine;
    private Coroutine layer4Coroutine;
    public void Enter(InteractionState.InteractionType preState)
    {
        switch (preState)
        {
            case InteractionState.InteractionType.UseItem: // Layer4
                IdleBlending = true;
                break;
            case InteractionState.InteractionType.Box: // Layer1
                IdleBlending = true;
                break;
            case InteractionState.InteractionType.Climb: // Layer1
                // layer1Coroutine = StartCoroutine(FromLayer1());
                IdleBlending = true;
                break;

        }
    }
    /*
    IEnumerator FromLayer1()
    {
        // Layer1 ->0
        float t = 0;

        while (t <= 1)
        {
            t += Time.deltaTime * 6;

            con.Animation.SetLayerWeight(1, t);
            yield return null;
        }
        con.Animation.SetLayerWeight(1, 0);
        yield return null;
    }
    */
    IEnumerator FromLayer4()
    {
        
        yield return null;
    }

    public void RequestStopLayer1()
    {
        if (layer1Coroutine != null)
            StopCoroutine(layer1Coroutine);
        IdleBlending = false;
    }
    public void RequestStopLayer4()
    {
        if (layer4Coroutine != null)
            StopCoroutine(layer4Coroutine);
        IdleBlending = false;

    }

}
