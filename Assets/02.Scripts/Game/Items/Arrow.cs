using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class Arrow : MonoBehaviour
{
    private float gravityMultiplier;
    private ArrowPool pool;
    private Rigidbody rb;
    private bool stuck = false;
    float airDrag = 0.1f;
    float airBoost = 0.15f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {

        Vector3 v = rb.linearVelocity;

        if (v.sqrMagnitude > 0.1f)
        {
            transform.forward = Vector3.Lerp(
                transform.forward,
                v.normalized,
                Time.deltaTime * 15f
            );
        }
    }
    private void FixedUpdate()
    {
        if (!stuck)
        {
            float gravityScale = rb.linearVelocity.y > 0 ? 2.5f : 7f;

            rb.AddForce(
                Physics.gravity * gravityScale,
                ForceMode.Acceleration
            );
            // 공기 저항
            rb.linearVelocity *= 1f / (1f + airDrag * Time.fixedDeltaTime);

        }
    }
    public void Shoot(ArrowPool arrowPool, Vector3 dir, float force)
    {
        Debug.Log($"dir: {dir}, force: {force}");
        pool = arrowPool;

        stuck = false;

        transform.SetParent(null);

        rb.isKinematic = false;
        rb.detectCollisions = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 shootDir =
    (dir + Vector3.up * airBoost).normalized;

        rb.linearVelocity = shootDir * force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log($"Hit: {collision.gameObject.name}, Tag: {collision.gameObject.tag}");
        if (stuck)
            return;

        stuck = true;

        CancelInvoke();

        ContactPoint contact = collision.contacts[0];

        transform.position = contact.point - transform.forward * 0.1f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.detectCollisions = false;

        transform.SetParent(collision.transform);

        Invoke(nameof(ReturnToPool), 10f);
    }

    void ReturnToPool()
    {
        CancelInvoke();

        transform.SetParent(null);

        rb.isKinematic = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        pool.ReturnArrow(gameObject);
    }
}