using UnityEngine;

public class PlayerController
{
    // Components (MonoBehavior)
    public InputManager Input { get; }
    public CameraFollow3D Cam { get; }
    public Player Player { get; }
    public PlayerStateMachine StateMachine { get; }
    public GroundCheck GroundCheck { get; }
    public PlayerTrigger Trigger { get; }
    public CharacterController cc { get; }
    public InteractionState Interaction { get; }
    public LocomotionState Locomotion { get; }
    public Parrying Parrying { get; }
    public Dodge Dodge { get; }
    public Animator Animator { get; }
    public Attack Attack { get; }
    public AttackState AttackState { get; }
    public AnimationEventController AnimationEventController { get; }

    // System (순수 C#)
    public DeadState Dead { get; }
    public PlayerMovement Movement { get; }
    public KnockbackState Knockback { get; }
    public Health Health { get; }
    public ActionState ActionState { get; }
    public EventManager Event { get; }
    public AnimationController Animation { get; }

    // 엄
    public PlayerController(InputManager input,
        Player player,
        PlayerStateMachine stateMachine,
        InteractionState interactionState,
        GroundCheck groundCheck,
        CameraFollow3D cam,
        CharacterController characterController,
        Dodge dodge,
        Animator animator,
        Attack attack,
        AttackState attackState,
        AnimationEventController animationEvent)
    {
        Input = input;
        Player = player;
        StateMachine = stateMachine;
        Attack = attack;
        Interaction = interactionState;
        GroundCheck = groundCheck;
        Cam = cam;
        cc = characterController;
        Dodge = dodge;
        Animator = animator;
        AttackState = attackState;
        AnimationEventController = animationEvent;

        // 순수 C# 코드 생성
        Dead = new DeadState(this);
        Knockback = new KnockbackState(this);
        Movement = new PlayerMovement(this);
        Health = new Health(this);
        ActionState = new ActionState(this);
        Event = new EventManager(this);
        Animation = new AnimationController(this);

        // Input에 직접 넣어줌
        input.Init(this);
    }
}
