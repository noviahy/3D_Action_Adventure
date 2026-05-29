using UnityEngine;

public class GroundCheck : PlayerBehaviour
{
    [SerializeField] private float distance = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    public bool IsGrounded { get; private set; }
    private void Update()
    {
        IsGrounded = Physics.Raycast(transform.position, Vector3.down, distance, groundLayer);
        con.Animation.SetGrounded(IsGrounded);
        
    }

}
