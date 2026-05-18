using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private int poolSize = 15;
    private float ShootForce = 30f;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab);

            arrow.SetActive(false);

            pool.Enqueue(arrow);
        }
    }

    public GameObject GetArrow(Vector3 dir, Transform firePoint, float force)
    {
        GameObject obj;

        if (pool.Count == 0)
            obj = Instantiate(arrowPrefab);
        else
            obj = pool.Dequeue();

        // 위치
        obj.transform.position = firePoint.position;

        // 방향
        obj.transform.rotation = Quaternion.LookRotation(dir);

        obj.SetActive(true);

        Arrow objCode = obj.GetComponent<Arrow>();
        objCode.Shoot(this, dir, force * ShootForce);

        return obj;
    }

    public void ReturnArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        pool.Enqueue(arrow);
    }
}
