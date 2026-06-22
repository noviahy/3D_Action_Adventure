using UnityEngine;
public class EdgeCheck : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;

    public float EdgeValue { get; private set; }
    public bool HasGround { get; private set; }
    public Vector3 Floor {  get; private set; }

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 10f, groundLayer))
        {
            Floor = hit.point;
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
