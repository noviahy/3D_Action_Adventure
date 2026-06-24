using Unity.Cinemachine;
using UnityEngine;

public class CameraDistance {
    private readonly CinemachineThirdPersonFollow follow;

    private readonly float normalDistance;
    private readonly float bowOffset;
    private readonly float pitchDistanceOffset;

    public CameraDistance(
        CinemachineThirdPersonFollow follow,
        float normalDistance,
        float bowOffset,
        float pitchDistanceOffset)
    {
        this.follow = follow;
        this.normalDistance = normalDistance;
        this.bowOffset = bowOffset;
        this.pitchDistanceOffset = pitchDistanceOffset;
    }

    public void UpdateDistance(float pitch, float minPitch, bool isBowAim)
    {
        float targetDistance = normalDistance;

        float pitchT = Mathf.InverseLerp(0f, minPitch, pitch);

        targetDistance += pitchDistanceOffset * pitchT;

        if (isBowAim)
            targetDistance += bowOffset;

        follow.CameraDistance = Mathf.Lerp(
            follow.CameraDistance,
            targetDistance,
            Time.deltaTime * 6f);
    }
}
