using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private Transform topArrivePoint;
    [SerializeField] private Transform enterPoint;
    [SerializeField] private Collider col;

    private void Start()
    {
        Vector3 top = col.bounds.center + Vector3.up * col.bounds.extents.y;
        Vector3 bottom = col.bounds.center - Vector3.up * col.bounds.extents.y;

        topArrivePoint.position = top + transform.forward * 0.7f;
        enterPoint.position = bottom + transform.forward * 0.5f;
    }
}
