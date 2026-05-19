using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private LayerMask hitMask;
    private ArrowPool pool;
    private Rigidbody rb;
    private bool stuck = false;
    float airDrag = 0.8f;
    float airBoost = 0.15f;
    float maxSpeed = 30f;
    Vector3 prevPos;
    private bool justShot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // 프레임 사이 움직임 부드럽게 보간
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    private void Update()
    {
        Debug.Log(rb.linearVelocity);
    }
    private void FixedUpdate()
    {
        // 첫 발사 프레임 예외 처리
        if (justShot)
        {
            // 현재 위치 저장
            prevPos = transform.position;
            justShot = false;
            return;
        }
        if (stuck) return;

        // 이동 방향 계산 코드
        Vector3 currentPos = transform.position;
        // 이전 위치 -> 현재 위치 방향
        Vector3 moveDir = rb.linearVelocity.normalized;
        // 이번 프레임 동안 이동할 거리
        float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;

        // 충돌 검사
        // 화살이 이번 프레임 동안 지나갈 구간 검사
        // Raycast기 때문에 관통 안 함
        if (distance > 0.001f)
        {
            if (Physics.Raycast(
                prevPos, // 시작점
                moveDir, // 방향
                out RaycastHit hit, // out hit
                distance, // 거리
                hitMask, // 레이어마스크
                QueryTriggerInteraction.Ignore))
            {
                OnHit(hit);
                return;
            }
        }
        // 중력
        float gravityScale = rb.linearVelocity.y > 0 ? 0f : 0.65f;

        rb.AddForce(
            Physics.gravity * gravityScale,
            ForceMode.Acceleration
        );
        // 공기 저항
        rb.linearVelocity *= 1f / (1f + airDrag * Time.fixedDeltaTime);

        // 최대 속도 조절
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;

        prevPos = currentPos;
        
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            transform.rotation =
                Quaternion.LookRotation(
                    rb.linearVelocity.normalized,
                    Vector3.up
                );
        }
    }
    public void Shoot(ArrowPool arrowPool, Vector3 dir, float force)
    {
        // 첫 프레임 확인용
        justShot = true;
        pool = arrowPool;

        // 전 상태 초기화
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.SetParent(null);

        // 
        transform.localScale = Vector3.one;
        transform.rotation = transform.rotation =
    Quaternion.LookRotation(dir, Vector3.up);

        stuck = false;


        // 물리 활성화
        rb.isKinematic = false;

        // 방향 계산
        Vector3 shootDir = (dir + Vector3.up * airBoost).normalized;

        rb.linearVelocity = shootDir * force;

        prevPos = transform.position;
        Debug.Log(dir);

    }

    void ReturnToPool()
    {
        CancelInvoke();

        transform.SetParent(null);

        pool.ReturnArrow(gameObject);
    }
    private void OnHit(RaycastHit hit)
    {
        if (stuck) return;

        stuck = true;


        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // 물리 비활성화
        rb.isKinematic = true;

        // 박힌 위치로 이동
        transform.position = hit.point - transform.forward * 0.05f;

        // 맞은 대상에 붙임
        transform.SetParent(hit.collider.transform);

        // 5초 후 Pool로 return
        Invoke(nameof(ReturnToPool), 5f);
    }
}