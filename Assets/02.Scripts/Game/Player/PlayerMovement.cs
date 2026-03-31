using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;

    public void Walk(Vector3 dir)
    {

    }
    public void Run(Vector3 dir) { }
    public void Jump() { }
}
