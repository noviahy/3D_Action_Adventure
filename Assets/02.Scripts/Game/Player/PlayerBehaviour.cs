using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    protected PlayerController con;

    public virtual void Init(PlayerController controller)
    {
        con = controller;
    }
}
