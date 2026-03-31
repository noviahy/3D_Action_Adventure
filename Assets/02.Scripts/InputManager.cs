using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private CameraFollow3D cam;
    public Vector3 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool RunPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool BowPressed { get; private set; }
    public bool BowReleased { get; private set; }
    public bool isLockOn { get; private set; }

    public float mouseX { get; private set; }
    public float mouseY { get; private set; }
    public InputMode CurrentInput { get; private set; }

    public enum InputMode
    {
        PlayerInput,
        UIInput,
        InputLock
    }
    public void ChangeInputMode(InputMode mode)
    {
        if (CurrentInput == mode) return;

        CurrentInput = mode;
    }

    void Update()
    {
        BowReleased = false;

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");


        float forward = Input.GetAxisRaw("Vertical");
        float side = Input.GetAxisRaw("Horizontal");
        MoveInput = cam.camForward * forward + cam.camRight * side;

        AttackPressed = Input.GetKeyDown("Attack");

        RunPressed = Input.GetButtonDown("Run");

        ParryingPressed = Input.GetButtonDown("Parrying");

        RollPressed = Input.GetKeyDown("Roll");

        BowPressed = Input.GetButtonDown("Bow");
        BowReleased = Input.GetButtonUp("Bow");
    }
}
