using UnityEngine;

public class CameraFollow3D : MonoBehaviour
{
    [SerializeField] Transform cameraRoot;
    [SerializeField] Transform cameraPivot;
    [SerializeField] InputManager input;
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform player;

    public Vector3 camForward { get; private set; }
    public Vector3 camRight { get; private set; }
    private float yaw;
    private float pitch;


    void LateUpdate()
    {
        yaw += input.mouseX * sensitivity * Time.deltaTime;
        pitch -= input.mouseY * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -40f, 70f);

        cameraRoot.rotation = Quaternion.Euler(0, yaw, 0);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0, 0);

        camForward = new Vector3(cameraRoot.forward.x, 0, cameraRoot.forward.z).normalized;
        camRight = new Vector3(cameraRoot.right.x, 0, cameraRoot.right.z).normalized;

        camForward.Normalize();
        camRight.Normalize();
    }
}
