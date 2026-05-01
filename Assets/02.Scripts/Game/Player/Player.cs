using UnityEngine;

public class Player : MonoBehaviour
{
    // Player에서 사용하는 모든 MonoBehavior를 모아 PlayerController로 보내줌
    [SerializeField] InputManager input;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] AttackState attack;
    [SerializeField] InteractionState interaction;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] CameraFollow3D cam;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] Parrying parrying;
    [SerializeField] Dodge dodge;

    private PlayerController Controller;

    // 매 Attack마다 ChangeAttackType를 해줘야
    // 무기 바꿀 때 바꾼지 확인 가능
    // 이거 옮겨줘야할듯
    public AttackType currentAttackType { get; private set; }
    public AttackType previousAttackType { get; private set; }
    public bool IsInvincible { get; private set; }

    public enum AttackType
    {
        Sword,
        Bow,
        Bomb
    }
    private void Awake()
    {
        // Controller 생성
        Controller = new PlayerController(
            input, 
            this, 
            stateMachine, 
            attack,
            interaction,
            groundCheck, 
            cam,  
            characterController,
            dodge,
            animator
        );

    }
    private void Start()
    {
        var behaviours = GetComponentsInChildren<PlayerBehaviour>();

        foreach (var b in behaviours)
        {
            b.Init(Controller);
        }
    }

    public void ChangeAttackType(AttackType type)
    {
        if (currentAttackType == type) return;

        previousAttackType = currentAttackType;
        currentAttackType = type;
    }
    public ItemType CurrentItemType {  get; private set; }
        public enum ItemType
    {
        HPPosion,
        Bomb
    }
    public void ChangeInvincible(bool value)
    {
        IsInvincible = value;
    }

}
