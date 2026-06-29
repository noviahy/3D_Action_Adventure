using System.Collections;
using UnityEngine;

public class ActionIdle : PlayerBehaviour
{
    public void Enter(ActionState.ActionType preState)
    {
        switch (preState)
        {
            case ActionState.ActionType.Dodge:
                StartCoroutine(OffLayer1());
                break;
            case ActionState.ActionType.Attack:
                if (con.Player.currentWeaponType == Player.WeaponType.Sword)
                    StartCoroutine(OnLayer2());
                break;
        }
    }
    IEnumerator OnLayer2() // ·¹ÀÌ¾î 1 ²ô°í 2 ÄÔ
    {
        // Layer1: 1 -> 0
        // Layer2: 0 -> 1
        con.LayerController.RequestLayer2On(0.2f);

        while (con.LayerController.ValueLayer2 >= 1f)
            yield return null;
        StartCoroutine(OffLayer1());
    }
    IEnumerator OffLayer1() // ·¹ÀÌ¾î 1 ²û
    {
        con.LayerController.RequestLayer1Off(0.5f);

        while (con.LayerController.ValueLayer1 != 0)
            yield return null;

        yield return null;

    }
}
