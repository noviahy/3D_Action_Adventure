using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class LayerController : PlayerBehaviour
{
    public float ValueLayer1 { get; private set; }
    public float ValueLayer2 { get; private set; }
    public float ValueLayer3 { get; private set; }

    private Coroutine layer1On;
    private Coroutine layer2On;
    private Coroutine layer3On;
    private Coroutine layer1Off;
    private Coroutine layer2Off;
    private Coroutine layer3Off;


    public void RequestLayer1On(float duration)
    {
        // 이미 켜져있거나 켜는 중이면 무시
        if (ValueLayer1 >= 1f || layer1On != null)
            return;

        // 끄는 중이었다면 중단
        if (layer1Off != null)
        {
            StopCoroutine(layer1Off);
            layer1Off = null;
        }
        layer1On = StartCoroutine(LayerOn(1, duration));
    }

    public void RequestLayer2On(float duration)
    {
        if (ValueLayer2 >= 1f || layer2On != null)
            return;

        if (layer2Off != null)
        {
            StopCoroutine(layer2Off);
            layer2Off = null;
        }
        layer2On = StartCoroutine(LayerOn(2, duration));
    }
    public void RequestLayer3On(float duration)
    {
        if (ValueLayer3 >= 1f || layer3On != null)
            return;

        if (layer3Off != null)
        {
            StopCoroutine(layer3Off);
            layer3Off = null;
        }
        layer3On = StartCoroutine(LayerOn(3, duration));
    }

    public void RequestLayer1Off(float duration)
    {
        // 이미 꺼져있거나 끄는 중이면 무시
        if (ValueLayer1 <= 0f || layer1Off != null)
            return;

        // 켜는 중이었다면 중단
        if (layer1On != null)
        {
            StopCoroutine(layer1On);
            layer1On = null;
        }

        layer1Off = StartCoroutine(LayerOff(1, duration));
    }
    public void RequestLayer2Off(float duration)
    {
        if (ValueLayer2 <= 0f || layer2Off != null)
            return;

        if (layer2On != null)
        {
            StopCoroutine(layer2On);
            layer2On = null;
        }

        layer2Off = StartCoroutine(LayerOff(2, duration));
    }
    public void RequestLayer3Off(float duration)
    {
        if (ValueLayer3 <= 0f || layer3Off != null)
            return;

        if (layer3On != null)
        {
            StopCoroutine(layer3On);
            layer3On = null;
        }

        layer3Off = StartCoroutine(LayerOff(3, duration));
    }

    IEnumerator LayerOn(int layer, float duration)
    {
        float start = GetLayerValue(layer);
        float end = 1f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;
            t = Mathf.Clamp01(t);

            float value = Mathf.Lerp(start, end, t);

            con.Animation.SetLayerWeight(layer, value);
            SetLayerValue(layer, value);

            yield return null;
        }
        con.Animation.SetLayerWeight(layer, 1);
        SetLayerValue(layer, 1);
        SetOnCoroutineNull(layer);
    }
    IEnumerator LayerOff(int layer, float duration)
    {
        float start = GetLayerValue(layer); // 현재 값
        float end = 0f;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            float value = Mathf.Lerp(start, end, t);

            SetLayerValue(layer, value);
            con.Animation.SetLayerWeight(layer, value);

            yield return null;
        }

        SetLayerValue(layer, 0f);
        con.Animation.SetLayerWeight(layer, 0f);
        SetOffCoroutineNull(layer);
    }
    float GetLayerValue(int layer)
    {
        switch (layer)
        {
            case 1: return ValueLayer1;
            case 2: return ValueLayer2;
            case 3: return ValueLayer3;
        }

        return 0f;
    }
    void SetOnCoroutineNull(int layer)
    {
        switch (layer)
        {
            case 1: layer1On = null ; break;
            case 2: layer2On = null; break;
            case 3: layer3On = null; break;
        }
    }
    void SetOffCoroutineNull(int layer)
    {
        switch (layer)
        {
            case 1: layer1Off = null; break;
            case 2: layer2Off = null; break;
            case 3: layer3Off = null; break;
        }
    }
    void SetLayerValue(int layer, float value)
    {
        switch (layer)
        {
            case 1: ValueLayer1 = value; break;
            case 2: ValueLayer2 = value; break;
            case 3: ValueLayer3 = value; break;
        }
    }
}
