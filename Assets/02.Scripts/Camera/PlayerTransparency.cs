using System.Collections.Generic;
using UnityEngine;

public class PlayerTransparency
{
    private List<Material> mats = new List<Material>();

    public void SetAlpha(float alpha)
    {
        foreach (var mat in mats)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.SetColor("_BaseColor", c);
        }
    }

    public void RefreshRenderers(Transform player)
    {
        mats.Clear();

        Renderer[] renderers = player.GetComponentsInChildren<Renderer>(true);

        foreach (var ren in renderers)
        {
            foreach (var mat in ren.materials)
            {
                mats.Add(mat);
            }
        }
    }

}
