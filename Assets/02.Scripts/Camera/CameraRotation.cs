using UnityEngine;

public class CameraRotation
{
    private float yaw;
    private float pitch;
    public float Pitch => pitch;

    private readonly float minPitch;
    private readonly float maxPitch;

    public CameraRotation(float minPitch, float maxPitch)
    {
        this.minPitch = minPitch;
        this.maxPitch = maxPitch;
    }

    public void RotateFree(
        Transform pivot,
        float mouseX,
        float mouseY,
        float speed)
    {
        yaw += mouseX * speed * Time.deltaTime;
        pitch += mouseY * speed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    public void RotateLockOn(
        Transform pivot,
        Transform target)
    {
        Vector3 dir = (target.position - pivot.position).normalized;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        pivot.rotation = Quaternion.Slerp(
            pivot.rotation,
            targetRot,
            10f * Time.deltaTime
        );

        Vector3 angles = pivot.rotation.eulerAngles;

        pitch = angles.x;
        yaw = angles.y;
    }
}
