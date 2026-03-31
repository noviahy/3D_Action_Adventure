using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private CameraFollow3D cam;
    public Vector3 MoveInput { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool LightAttack {  get; private set; }
    public bool HeavyAttack {  get; private set; }
    public bool RunPressed { get; private set; }
    public bool ParryingPressed { get; private set; }
    public bool RollPressed { get; private set; }
    public bool BowPressed { get; private set; }
    public bool BowReleased { get; private set; }
    public bool isLockOn { get; private set; }
    public bool InteractionPressed {  get; private set; }
    public bool ChangeItem {  get; private set; }

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

    void Update() // 동시에 누를때 생기는 문제 처리해야함
    {
        BowReleased = false;

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");


        float forward = Input.GetAxisRaw("Vertical");
        float side = Input.GetAxisRaw("Horizontal");
        MoveInput = cam.camForward * forward + cam.camRight * side;

        AttackPressed = Input.GetKeyDown("Attack");
        LightAttack = Input.GetKeyDown("Light");
        HeavyAttack = Input.GetKeyDown("Heavy");

        ChangeItem = Input.GetKeyDown("ChangeItem");
        InteractionPressed = Input.GetKeyDown("Interaction");

        RunPressed = Input.GetButtonDown("Run");

        ParryingPressed = Input.GetButtonDown("Parrying");

        RollPressed = Input.GetKeyDown("Roll");

        BowPressed = Input.GetButtonDown("Bow");
        BowReleased = Input.GetButtonUp("Bow");
    }
}
