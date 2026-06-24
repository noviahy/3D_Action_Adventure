using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BowCamera
{
    public void RotatePlayer(Transform player, Vector3 camForward)
    {
        Vector3 dir = camForward;
        dir.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        player.rotation = Quaternion.Slerp(
            player.rotation,
            targetRot,
            15f * Time.deltaTime);
    }

    public void UpdateAimTarget(Transform cam, Transform aimTarget)
    {
        aimTarget.position = cam.position + cam.forward * 10f;
    }

    public void UpdateRig(Rig upperBodyRig, bool bowShoot)
    {
        float targetWeight = bowShoot ? 0f : 1f;

        upperBodyRig.weight = Mathf.Lerp(
            upperBodyRig.weight,
            targetWeight,
            Time.deltaTime * 10f);
    }
}
