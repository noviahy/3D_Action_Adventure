using UnityEngine;
public class EdgeCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    public float EdgeValue { get; private set; }
    public bool HasGround { get; private set; }

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            HasGround = true;
            EdgeValue = transform.position.y - hit.point.y;
        }
        else
        {
            HasGround = false;
            EdgeValue = float.MaxValue;
        }
    }
}
